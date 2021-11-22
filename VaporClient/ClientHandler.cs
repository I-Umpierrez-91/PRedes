using Common;
using Common.FileHandler;
using Common.FileHandler.Interfaces;
using Common.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using ProtocolLibrary;
using System;
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
        private IFileStreamHandler _fileStreamHandler = new FileStreamHandler();
        public async Task ClientHandlerStart()
        {
            var clientEndPoint = new IPEndPoint(IPAddress.Parse(SettingsMgr.ReadSetting(ClientConfig.ClientIpConfigKey))
                , int.Parse(SettingsMgr.ReadSetting(ClientConfig.ClientPortConfigKey)));
            _tcpClient = new TcpClient(clientEndPoint);
            try
            {
                await _tcpClient.ConnectAsync(
                    IPAddress.Parse(SettingsMgr.ReadSetting(ClientConfig.ServerIpConfigKey)),
                    int.Parse(SettingsMgr.ReadSetting(ClientConfig.SeverPortConfigKey))).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        public async Task SendParts(string path)
        {
            var _fileStreamHandler = new FileStreamHandler();
            var fileParts = _fileStreamHandler.GetFileParts(path, HeaderConstants.MaxPacketSize);
            foreach (var i in fileParts)
            {
                await _networkStreamHandler.Write(i);
            }
        }

        public async Task<string> ReadResponse()
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

        public async Task<string> ReceiveFile()
        {
            string workingDirectory = Environment.CurrentDirectory;

            string fileInfo = await ReadResponse();
            string[] values = fileInfo.Split(HeaderConstants.Divider);

            string fileName = values[0];
            string fileSize = values[1];

            if (int.Parse(fileSize) > 0)
            {
                await _fileStreamHandler.ReceiveFile(fileName, int.Parse(fileSize), HeaderConstants.MaxPacketSize, _networkStreamHandler);
                string receivePath = workingDirectory + "\\" + fileName;
                return "La carátula fue descargada en: " + receivePath;
            }
            else
            {
                return "El juego no tiene carátula";
            }
        }
    }
}
