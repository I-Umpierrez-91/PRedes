using Common.NetworkUtils.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.FileHandler.Interfaces
{
    public interface IFileStreamHandler
    {
        byte[] Read(string path, long offset, int length);
        void Write(string fileName, byte[] data, int? round);
        public Task ReceiveFile(string fileName, int fileSize, int maxPacketSize, INetworkStreamHandler networkStreamHandler);
        public List<byte[]> GetFileParts(string path, int maxPacketSize);
    }
}