using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace VaporClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var isRunning = true;
            var clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
            var serverEndPoint = new IPEndPoint(SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey)),
                int.Parse(SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey)));
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(clientEndPoint);
            socket.Connect(serverEndPoint);
            Console.WriteLine("Conectado al servidor");
            try
            {
                while (isRunning)
                {
                    var message = Console.ReadLine();
                    var data = Encoding.UTF8.GetBytes(message);
                    if (data.Equals("exit"))
                    {
                        isRunning = false;
                    }
                    var dataSent = socket.Send(data);
                    Console.WriteLine("Bytes sent {0}", dataSent);
                }
            }
            catch (SocketException ex)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                Console.WriteLine("El servidor cerró la conexión");
                Console.ReadLine();
            }
        }
    }
}