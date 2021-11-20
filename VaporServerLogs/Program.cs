using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Common;
using Common.Interfaces;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace VaporServerLogs
{
    public class Program
    {
        private static ISettingsManager SettingsMgr = new SettingsManager();
        static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            using var channel = new ConnectionFactory() {HostName = SettingsMgr.ReadSetting(ServerConfig.LogQueueIpConfigKey)}.CreateConnection().CreateModel();
            channel.QueueDeclare(queue: "log_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var log = JsonSerializer.Deserialize<Log>(message);
                Console.WriteLine(" [x] Received log level [{0}], message [{1}]", log.Level, log.Message);
            };
            channel.BasicConsume(queue: "log_queue",
                autoAck: true,
                consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
