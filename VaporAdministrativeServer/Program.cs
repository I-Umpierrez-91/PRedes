using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using Grpc.Net.Client;
using System.Threading.Tasks;
using Common.Interfaces;
using Common;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace VaporAdministrativeServer
{
    public class Program
    {
        private static readonly ISettingsManager SettingsMgr = new SettingsManager();
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            Console.WriteLine("Starting gRPC client example......");
            var channel = GrpcChannel.ForAddress(SettingsMgr.ReadSetting(ServerConfig.ServerAddressConfigKey)); 
            var userClient = new UserService.UserServiceClient(channel);
            var gameClient = new GameService.GameServiceClient(channel);

            Logic.initializeLogic(userClient, gameClient);

            CreateHostBuilder(args).Build().Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(int.Parse(SettingsMgr.ReadSetting(ServerConfig.AdmServerPortConfigKey)), o => o.Protocols = HttpProtocols.Http1AndHttp2);
            });
            webBuilder.UseStartup<Startup>();
        });
    }
}
