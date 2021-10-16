using Common;
using Common.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using ProtocolLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VaporServer.Interfaces;

namespace VaporServer
{
    class ServerHandler
    {
        private readonly ISettingsManager SettingsMgr = new SettingsManager();
        static List<Socket> _clients = new List<Socket>();
        private static int _clientNumber;
        private int _isTestDataLoaded;
        private static ILogic _logic = new Logic();
        static bool _exit = false;
        private readonly Socket _server;

        public ServerHandler()
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEp = new IPEndPoint(
                IPAddress.Parse(SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey)),
                int.Parse(SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey)));
            _server.Bind(localEp);
            _server.Listen(100);
            try
            {
                var threadServer = new Thread(() => ListenForConnections(_server));
                threadServer.Start();
            }
            catch (Exception)
            {

                Console.WriteLine("Cerrando....");
            }
        }

        public void CloseConnections()
        {
            foreach (var client in _clients)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            _server.Close(0);
        }

    private void ListenForConnections(Socket socketServer)
        {
            while (!_exit)
            {
                try
                {
                    var clientConnected = socketServer.Accept();
                    _clients.Add(clientConnected);
                    var threacClient = new Thread(() => HandleClient(clientConnected));
                    threacClient.Start();
                }
                catch (Exception e)
                {
                    //Console.WriteLine("");
                    _exit = true;
                }
            }
            Console.WriteLine("Cerrando....");
        }

        private static void HandleClient(Socket client)
        {
            var id = Interlocked.Add(ref _clientNumber, 1);
            var connected = true;
            Console.WriteLine("Conectado el cliente " + id);
            var networkStreamHandler = new NetworkStreamHandler(new NetworkStream(client));
            //var networkStream = new NetworkStream(client);
            while (connected && !_exit)
            {
                try
                {
                    var headerLength = Header.GetLength();
                    byte[] buffer;
                    buffer = networkStreamHandler.Read(headerLength);
                    var header = new Header();
                    header.DecodeData(buffer);
                    switch (header.ICommand)
                    {
                        case CommandConstants.ListGames:
                            var resMessage = Encoding.UTF8.GetBytes(_logic.PrintGameList());
                            var resHeader = new Header(HeaderConstants.Response, CommandConstants.Message, resMessage.Length);
                            networkStreamHandler.Write(resHeader.GetResponse());
                            networkStreamHandler.Write(resMessage);
                            break;
                        case CommandConstants.ShowGameDetails:                            
                            byte[] bufferData = networkStreamHandler.Read(header.IDataLength);
                            var resMessage2 = Encoding.UTF8.GetBytes(_logic.PrintGameDetails(Encoding.UTF8.GetString(bufferData)));

                            Console.WriteLine("El cliente idicó que quiere ver el juego con id " + Encoding.UTF8.GetString(bufferData));

                            var resHeader2 = new Header(HeaderConstants.Response, CommandConstants.Message, HeaderConstants.DataLength);
                            networkStreamHandler.Write(resHeader2.GetResponse());
                            networkStreamHandler.Write(resMessage2);
                            break;
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine("El cliente " + id + " cerró la conexión: ");
                    connected = false;
                }
            }
        }
    }
}
