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
        private TcpClient _tcpClient;
        private INetworkStreamHandler _networkStreamHandler;
        public async Task ClientHandlerStart()
        {
            var clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
            _tcpClient = new TcpClient(clientEndPoint);

            await _tcpClient.ConnectAsync(
                IPAddress.Parse(SettingsMgr.ReadSetting(ServerConfig.ServerIpConfigKey)),
                int.Parse(SettingsMgr.ReadSetting(ServerConfig.SeverPortConfigKey))).ConfigureAwait(false);
            var keepConnection = true;

            _networkStreamHandler = new NetworkStreamHandler(_tcpClient.GetStream());
            Console.WriteLine("Conectado al servidor");
        }

        public async Task CloseConnection()
        {
            _tcpClient.Close();
        }

        public async Task SendRequest(int CommandConstant, string message)
        {
            var header = new Header(HeaderConstants.Request, CommandConstant, message.Length);
            var data = header.GetRequest();
            await _networkStreamHandler.Write(data);
            await _networkStreamHandler.Write(Encoding.UTF8.GetBytes(message));
        }

        public async Task< string> ReadResponse()
        {
            var headerLength = Header.GetLength();
            byte[] buffer;
            buffer = await _networkStreamHandler.Read(headerLength);
            var header = new Header();
            header.DecodeData(buffer);
            byte[] bufferData = await _networkStreamHandler.Read(header.IDataLength);

            var result = Encoding.UTF8.GetString(bufferData);

            return result;
        }
    }
}
