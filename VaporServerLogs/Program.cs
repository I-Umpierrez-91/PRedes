using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Common;
using Common.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace VaporServerLogs
{
    public class Program
    {
        static readonly ISettingsManager SettingsMgr = new SettingsManager();
        static void Main(string[] args)
        {
            Task.Run(()=>CreateHostBuilder(args).Build().Run());

            LogLogic.InitializeLogLogic();

            Console.WriteLine(" El servidor esta corriendo. Presione enter para apagarlo.");
            Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(int.Parse(SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey)), o => o.Protocols = HttpProtocols.Http1);
            });
            webBuilder.UseStartup<Startup>();
        });
    }
}


