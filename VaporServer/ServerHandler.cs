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
        private Socket _server;

        public async Task ServerHandlerStart()
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEp = new IPEndPoint(
                IPAddress.Parse(SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey)),
                int.Parse(SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey)));
            var tcpListener = new TcpListener(localEp);

            tcpListener.Start(100);

            while (true)
            {
                var tcpClientSocket = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                var task = Task.Run(async () => await HandleClient(tcpClientSocket).ConfigureAwait(false));
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

        private static async Task HandleClient(TcpClient client)
        {
            var id = Interlocked.Add(ref _clientNumber, 1);
            var connected = true;
            Console.WriteLine("Conectado el cliente " + id);
            var networkStreamHandler = new NetworkStreamHandler(client.GetStream());
            //var networkStream = new NetworkStream(client);
            while (connected && !_exit)
            {
                try
                {
                    var headerLength = Header.GetLength();
                    byte[] buffer;
                    buffer = await networkStreamHandler.Read(headerLength);
                    var header = new Header();
                    header.DecodeData(buffer);
                    switch (header.ICommand)
                    {
                        case CommandConstants.ListGames:
                            var resMessage = Encoding.UTF8.GetBytes(_logic.PrintGameList());
                            var resHeader = new Header(HeaderConstants.Response, CommandConstants.Message, resMessage.Length);
                            await networkStreamHandler.Write(resHeader.GetResponse());
                            await networkStreamHandler.Write(resMessage);
                            break;
                        case CommandConstants.ShowGameDetails:                            
                            byte[] bufferData = await networkStreamHandler.Read(header.IDataLength);
                            var resMessage2 = Encoding.UTF8.GetBytes(_logic.PrintGameDetails(Encoding.UTF8.GetString(bufferData)));

                            Console.WriteLine("El cliente idicó que quiere ver el juego con id " + Encoding.UTF8.GetString(bufferData));

                            var resHeader2 = new Header(HeaderConstants.Response, CommandConstants.Message, HeaderConstants.DataLength);
                            await networkStreamHandler.Write(resHeader2.GetResponse());
                            await networkStreamHandler.Write(resMessage2);
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
