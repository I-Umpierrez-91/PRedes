using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Common;
using Common.Interfaces;
using System.Threading.Tasks;

namespace VaporServerLogs
{
    public class Program
    {
        static void Main(string[] args)
        {
            Task.Run(()=>CreateHostBuilder(args).Build().Run()); 

            Console.WriteLine(" El servidor esta corriendo. Presione enter para apagarlo.");
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
