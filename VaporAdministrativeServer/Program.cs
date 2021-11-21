using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using Grpc.Net.Client;
using System.Threading.Tasks;

namespace VaporAdministrativeServer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            Console.WriteLine("Starting gRPC client example......");
            var channel = GrpcChannel.ForAddress("http://localhost:5001");
            var client = new UserService.UserServiceClient(channel);
            var response = await client.CreateUserAsync(new UserRequest
            {
                Username = "Probando gRPC",
                Password = "La concha de tu hermana"});
            Console.WriteLine("Respuesta: " + response.Message);

            Console.ReadLine();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });
    }
}
