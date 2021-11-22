using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Common;
using Common.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace VaporServerLogs
{
    public static class LogLogic
    {
        private static ISettingsManager SettingsMgr = new SettingsManager();

        private static bool exit = false;

        private static IModel _channel;

        private static string _queueName;

        private static Dictionary<string, List<Log>> _storedLogs = new Dictionary<string, List<Log>>();

        public static void InitializeLogLogic()
        {
            _channel = new ConnectionFactory() { HostName = SettingsMgr.ReadSetting(ServerConfig.LogQueueIpConfigKey) }.CreateConnection().CreateModel();
            _channel.ExchangeDeclare(exchange: "vapor_logs",
                        type: "topic",
                        durable: false,
                        autoDelete: false,
                        arguments: null);

            _queueName = _channel.QueueDeclare().QueueName;

            Task.Run(() => startLogReader("*.*.*"));
        }


        private static string GetRoutingKey(string gameId, string user, string date)
        {
            if (user.Length == 0)
            {
                user = "*";
            }
            if (gameId.Length == 0)
            {
                gameId = "*";
            }
            if (date.Length == 0)
            {
                date = "*";
            }
            return gameId + "." + user + "." + date;

        }

        public static async Task<IEnumerable<Log>> ReadLogs(string gameId, string user, string date)
        {
            var filteredLogs = new List<Log>();

            var routingKey = GetRoutingKey(gameId ,user ,date);

            bool found = _storedLogs.TryGetValue(routingKey, out filteredLogs);

            if (found)
            {
                return filteredLogs;
            }
            else
            {
                return new List<Log>() { new Log() { Level = "Info", Message = "No existe un binding para esa routing key. Para crear uno haga un post con la misma informacion." } };
            }

        }

        public static async Task<string> CreateLog(string gameId, string user, string date)
        {
            var routingKey = GetRoutingKey(gameId, user, date);

            bool found = _storedLogs.TryGetValue(routingKey, out var filteredLogs);

            if (found)
            {
                return "La cola de mensajes ya se esta leyendo.";
            }
            else
            {
                Task.Run(() => startLogReader(routingKey));
                return "Bind creado.";
            }

        }

        private static async Task startLogReader(string routingKey)
        {

            var LogList = new List<Log>();
            _storedLogs.Add(routingKey, LogList);

            _channel.QueueBind(queue: _queueName,
                        exchange: "vapor_logs",
                        routingKey: routingKey);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var log = JsonSerializer.Deserialize<Log>(message);
                _storedLogs.TryGetValue(routingKey, out var storedList );
                storedList.Add(log);
                Console.WriteLine(log.Message);
                ;
            };
            _channel.BasicConsume(queue: _queueName,
                autoAck: true,
                consumer: consumer);

            Console.WriteLine(" ");
            Console.ReadLine();
        }

    }
}
