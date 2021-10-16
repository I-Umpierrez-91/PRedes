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
using System.Threading.Tasks;

namespace VaporClient
{
    class ClientHandler
    {
        static readonly ISettingsManager SettingsMgr = new SettingsManager();
        private Socket _socket;
        private INetworkStreamHandler _networkStreamHandler;
        public ClientHandler()
        {
            var clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
            var serverEndPoint = new IPEndPoint(IPAddress.Parse(SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey)),
                int.Parse(SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey)));
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(clientEndPoint);
            _socket.Connect(serverEndPoint);
            _networkStreamHandler = new NetworkStreamHandler(new NetworkStream(_socket));
            Console.WriteLine("Conectado al servidor");
        }

        public void CloseConnection()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        public void SendRequest(int CommandConstant, string message)
        {
            var header = new Header(HeaderConstants.Request, CommandConstant, message.Length);
            var data = header.GetRequest();
            _networkStreamHandler.Write(data);
            _networkStreamHandler.Write(Encoding.UTF8.GetBytes(message));
        }

        public string ReadResponse()
        {
            var headerLength = Header.GetLength();
            byte[] buffer;
            buffer = _networkStreamHandler.Read(headerLength);
            var header = new Header();
            header.DecodeData(buffer);
            byte[] bufferData = _networkStreamHandler.Read(header.IDataLength);

            return Encoding.UTF8.GetString(bufferData);
        }
    }
}
