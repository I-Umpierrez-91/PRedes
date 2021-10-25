﻿using Common;
using Common.FileHandler;
using Common.FileHandler.Interfaces;
using Common.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using ProtocolLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        static BlockingCollection<TcpClient> _clients = new BlockingCollection<TcpClient>();
        static public IFileHandler _fileHandler = new FileHandler();
        static public IFileStreamHandler _fileStreamHandler = new FileStreamHandler();
        private static int _clientNumber;
        private int _isTestDataLoaded;
        private static ILogic _logic = new Logic();
        static bool _exit = false;

        public async Task ServerHandlerStart()
        {
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

        public async Task CloseConnections()
        {
            foreach (var client in _clients)
            {
                client.Dispose();
                client.Close();
            }
        }

        private static async Task HandleClient(TcpClient client)
        {
            var id = Interlocked.Add(ref _clientNumber, 1);
            _clients.Add(client);
            var connected = true;
            var logged = false;
            Console.WriteLine("Conectado el cliente " + id);
            var networkStreamHandler = new NetworkStreamHandler(client.GetStream());
            while (connected && !logged && !_exit)
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
                        case CommandConstants.Login:
                            byte[] bufferData = await networkStreamHandler.Read(header.IDataLength);
                            var loginData = Encoding.UTF8.GetString(bufferData);
                            string[] values = loginData.Split(HeaderConstants.Divider);

                            string username = values[0];
                            string password = values[1];

                            var result = _logic.Login(username, password);
                            logged = result;
                            var resMessage = Encoding.UTF8.GetBytes("400");
                            if (result)
                            {
                                resMessage = Encoding.UTF8.GetBytes("200");
                            }
                            var resHeader = new Header(HeaderConstants.Response, CommandConstants.Message, resMessage.Length);
                            await networkStreamHandler.Write(resHeader.GetResponse());
                            await networkStreamHandler.Write(resMessage);
                            break;
                        default:
                            break;
                    }

                }
                catch (SocketException ex)
                {
                    Console.WriteLine("El cliente " + id + " cerró la conexión");
                    _clients.TryTake(out client, TimeSpan.FromMilliseconds(1000));
                    connected = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error interno, cerrando la conexión. " + ex.GetType());
                    _clients.TryTake(out client, TimeSpan.FromMilliseconds(1000));
                    connected = false;
                }

            }


            while (connected && logged && !_exit)
            {
                try
                {
                    var headerLength = Header.GetLength();
                    byte[] buffer;
                    buffer = await networkStreamHandler.Read(headerLength);
                    var header = new Header();
                    Header resHeader;
                    header.DecodeData(buffer);
                    switch (header.ICommand)
                    {
                        case CommandConstants.ListGames:
                            Console.WriteLine("El cliente idicó que quiere ver la lista de juegos");

                            var resMessage = Encoding.UTF8.GetBytes(_logic.PrintGameList());
                            resHeader = new Header(HeaderConstants.Response, CommandConstants.Message, resMessage.Length);
                            await networkStreamHandler.Write(resHeader.GetResponse());
                            await networkStreamHandler.Write(resMessage);
                            break;
                        case CommandConstants.ShowGameDetails:                            
                            byte[] bufferData = await networkStreamHandler.Read(header.IDataLength);
                            var resMessage2 = Encoding.UTF8.GetBytes(_logic.PrintGameDetails(Encoding.UTF8.GetString(bufferData)));

                            Console.WriteLine("El cliente idicó que quiere ver el juego con id " + Encoding.UTF8.GetString(bufferData));

                            resHeader = new Header(HeaderConstants.Response, CommandConstants.Message, resMessage2.Length);
                            await networkStreamHandler.Write(resHeader.GetResponse());
                            await networkStreamHandler.Write(resMessage2);
                            break;
                        case CommandConstants.PublishGame:
                            Console.WriteLine("El cliente idicó que quiere publicar un juego");

                            byte[] publishBufferData = await networkStreamHandler.Read(header.IDataLength);
                            var publishMessage = Encoding.UTF8.GetString(publishBufferData);
                            string[] values = publishMessage.Split(HeaderConstants.Divider);

                            string name = values[0];
                            string genre = values[1];
                            string sinopsis = values[2];
                            string filename = values[3];
                            string fileSize = values[4];

                            string workingDirectory = Environment.CurrentDirectory;

                            string path = "";
                            if (!values[3].Equals(string.Empty))
                            {
                                path = workingDirectory + "\\" + filename;
                                await _fileStreamHandler.ReceiveFile(values[3], int.Parse(values[4]), HeaderConstants.MaxPacketSize, networkStreamHandler);
                            }

                            var resMessage3 = Encoding.UTF8.GetBytes(_logic.CreateGame(name, genre, sinopsis, path)) ;
                            resHeader = new Header(HeaderConstants.Response, CommandConstants.Message, resMessage3.Length);
                            await networkStreamHandler.Write(resHeader.GetResponse());
                            await networkStreamHandler.Write(resMessage3);


                            break;
                        case CommandConstants.SendFile:
                            Console.WriteLine("El cliente está enviando un archivo");

                            byte[] fileTransferBufferData = await networkStreamHandler.Read(header.IDataLength);
                            var fileInfo = Encoding.UTF8.GetString(fileTransferBufferData);
                            values = fileInfo.Split(HeaderConstants.Divider);

                            await _fileStreamHandler.ReceiveFile(values[0], int.Parse(values[1]), HeaderConstants.MaxPacketSize, networkStreamHandler);

                            break;
                        case CommandConstants.RequestGamePhoto:
                            Console.WriteLine("El cliente solicitó un archivo");
                            byte[] fileRequestBufferData = await networkStreamHandler.Read(header.IDataLength);
                            var fileRequestGameId = Encoding.UTF8.GetString(fileRequestBufferData);
                            var filePathToSend = _logic.GetGamePhotoPath(fileRequestGameId);
                            var div = HeaderConstants.Divider;
                            var photoResponseMessage = Encoding.UTF8.GetBytes((filePathToSend.Equals(string.Empty) ? "" : _fileHandler.GetFileName(filePathToSend)) + div +
                                (filePathToSend.Equals(string.Empty) ? "0" : _fileHandler.GetFileSize(filePathToSend).ToString()));

                            resHeader = new Header(HeaderConstants.Response, CommandConstants.Message, photoResponseMessage.Length);
                            await networkStreamHandler.Write(resHeader.GetResponse());
                            await networkStreamHandler.Write(photoResponseMessage);

                            var fileParts = _fileStreamHandler.GetFileParts(filePathToSend, HeaderConstants.MaxPacketSize);
                            foreach (var i in fileParts)
                            {
                                await networkStreamHandler.Write(i);
                            }

                            break;
                    }

                }
                catch (SocketException ex)
                {
                    Console.WriteLine("El cliente " + id + " cerró la conexión");
                    _clients.TryTake(out client, TimeSpan.FromMilliseconds(1000));
                    connected = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error interno, cerrando la conexión. " + ex.GetType());
                    _clients.TryTake(out client, TimeSpan.FromMilliseconds(1000));
                    connected = false;
                }
            }
        }
    }
}
