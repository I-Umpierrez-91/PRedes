using System;
using System.Text.Json;
using Common;
using Common.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace VaporServer
{
    public class LogHandler
    {
        private static ISettingsManager SettingsMgr = new SettingsManager();
        private static IModel channel;
        private static int logOption = 2; //1 = Conole only, 2 = Queue & Console, 3 = Queue Only
        private static bool _exit = false;
        public LogHandler()
        {
            channel = new ConnectionFactory() {HostName = SettingsMgr.ReadSetting(ServerConfig.LogQueueIpConfigKey)}.CreateConnection().CreateModel();
            channel.QueueDeclare(queue: "log_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

        }

        public void WriteLog(string severity, string LogMessage) {
            var log = new Log() {Level = severity, Message = LogMessage};
            switch (logOption) {
                case 1:
                    Console.WriteLine("Level: " + severity + " - Message: " + LogMessage);
                    break;
                case 2:
                    Console.WriteLine("Level: " + severity + " - Message: " + LogMessage);
                    Task.Run(() => SendMessage(log));
                    break;
                case 3:
                    Task.Run(() => SendMessage(log));
                    break;                
            }
        }

        public void SetLogOption(int option) {
            logOption = option;
        }

        public async Task<bool> SendMessage(Log message)
        {
            bool returnVal;
            var stringLog = JsonSerializer.Serialize(message);
            try
            {
                var body = Encoding.UTF8.GetBytes(stringLog);
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

            return returnVal;
        }

        public async Task StopLogQueue() {
            _exit = true;
        } 
    }
}
