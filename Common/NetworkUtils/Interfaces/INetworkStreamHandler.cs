using System.Threading.Tasks;

namespace Common.NetworkUtils.Interfaces
{
    public interface INetworkStreamHandler
    {
        Task Write(byte[] data);
        Task<byte[]> Read(int length);
    }
}