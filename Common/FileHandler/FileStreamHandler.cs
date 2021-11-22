using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common.FileHandler.Interfaces;
using Common.NetworkUtils.Interfaces;

namespace Common.FileHandler
{
    public class FileStreamHandler : IFileStreamHandler
    {
        private readonly IFileHandler _fileHandler = new FileHandler();
        public byte[] Read(string path, long offset, int length)
        {
            var data = new byte[length];

            using (var fs = new FileStream(path, FileMode.Open))
            {
                fs.Position = offset;
                var bytesRead = 0;
                while (bytesRead < length)
                {
                    var read = fs.Read(data, bytesRead, length - bytesRead);
                    if (read == 0)
                    {
                        throw new Exception("Couldn't not read file");
                    }
                    bytesRead += read;
                }
            }

            return data;
        }

        public void Write(string fileName, byte[] data, int? round)
        {
            if (File.Exists(fileName) && round != 1)
            {
                using (var fs = new FileStream(fileName, FileMode.Append))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            else
            {
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
        }

        public List<byte[]> GetFileParts(string path, int maxPacketSize)
        {
            List<byte[]> res = new List<byte[]>();
            long fileSize = _fileHandler.GetFileSize(path);
            string fileName = _fileHandler.GetFileName(path);

            long parts = fileSize / maxPacketSize;
            parts = parts * maxPacketSize == fileSize ? parts : parts + 1;

            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = Read(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = Read(path, offset, maxPacketSize);
                    offset += maxPacketSize;
                }
                res.Add(data);
                currentPart++;
            }
            return res;
        }

        public async Task ReceiveFile(string fileName, int fileSize, int maxPacketSize, INetworkStreamHandler networkStreamHandler)
        {
            long parts = fileSize / maxPacketSize;
            parts = parts * maxPacketSize == fileSize ? parts : parts + 1;

            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = await networkStreamHandler.Read(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = await networkStreamHandler.Read(maxPacketSize);
                    offset += maxPacketSize;
                }
                Write(fileName, data, ((int)currentPart));
                currentPart++;
            }
        }
    }
}