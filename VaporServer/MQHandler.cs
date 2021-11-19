using System;
using System.Text.Json;
using Common;
using Common.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace VaporServer
{
    public class MQHandler
    {
        private static ISettingsManager SettingsMgr = new SettingsManager();
        public async Task MQHandlerStart()
        {
            var channel = new ConnectionFactory() {HostName = SettingsMgr.ReadSetting(ServerConfig.LogQueueIpConfigKey)
                , Port = int.Parse(SettingsMgr.ReadSetting(ServerConfig.LogQueueConfigKey))}.CreateConnection().CreateModel();
            channel.QueueDeclare(queue: "log_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var exit = false;
            Console.WriteLine("Welcome to the MQ Async Sender!!");
            Console.WriteLine("Options:\n 1 - Send Message To Queue.\n 2 - Show Menu \n 3 - Exit");
            while (!exit)
            {
                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        var log = new Log();
                        Console.WriteLine("Enter Log Level:");
                        log.Level = Console.ReadLine();
                        Console.WriteLine("Enter message:");
                        log.Message = Console.ReadLine();
                        var stringLog = JsonSerializer.Serialize(log);
                        var result = await SendMessage(channel, stringLog);
                        Console.WriteLine(result ? "Message {0} sent successfully" : "Could not send {0}", stringLog);
                        Console.WriteLine("Options:\n 1 - Send Message To Queue.\n 2 - Show Menu \n 3 - Exit");
                        break;
                    case "2":
                        Console.WriteLine("Options:\n 1 - Send Message To Queue.\n 2 - Show Menu \n 3 - Exit");
                        break;
                    case "3":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Wrong Option Provided");
                        break;
                }
            }
        }

        private static Task<bool> SendMessage(IModel channel, string message)
        {
            bool returnVal;
            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "",
                    routingKey: "log_queue",
                    basicProperties: null,
                    body: body);
                returnVal = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                returnVal = false;
            }

            return Task.FromResult(returnVal);
        }
    }
}
