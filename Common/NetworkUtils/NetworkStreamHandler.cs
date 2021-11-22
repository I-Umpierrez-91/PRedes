using System.Net.Sockets;
using System.Threading.Tasks;
using Common.NetworkUtils.Interfaces;

namespace Common.NetworkUtils
{
    public class NetworkStreamHandler : INetworkStreamHandler
    {
        private readonly NetworkStream _networkStream;

        public NetworkStreamHandler(NetworkStream networkStream)
        {
            _networkStream = networkStream;
        }

        public async Task< byte[]> Read(int length)
        {
            int dataReceived = 0;
            var data = new byte[length];
            while (dataReceived < length)
            {
                var received = await _networkStream.ReadAsync(data, dataReceived, length - dataReceived).ConfigureAwait(false);
                if (received == 0)
                {
                    throw new SocketException();
                }
                dataReceived += received;
            }

            return data;
        }

        public async Task Write(byte[] data)
        {
            await _networkStream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
        }

    }
}