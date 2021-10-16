using Common;
using Common.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using ProtocolLibrary;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace VaporClient
{
    class Client
    {
        static readonly ISettingsManager SettingsMgr = new SettingsManager();

        static void Main(string[] args)
        {
            var isRunning = true;
            var _clientHandler = new ClientHandler();


            while (isRunning)
            {
                try
                {
                    
                    Console.WriteLine("--CLIENTE--");
                    Console.WriteLine("Opciones validas: ");
                    Console.WriteLine("0 -> Salir");
                    //Console.WriteLine("1 -> Publicar juego");
                    Console.WriteLine("2 -> Listar juegos");
                    Console.WriteLine("3 -> Detalle juego");
                    Console.WriteLine("Ingrese su opcion: ");
                    var option = Console.ReadLine();
                    switch (option)
                    {
                        case "0":
                            _clientHandler.CloseConnection();
                            isRunning = false;
                            break;
                        case "1":

                            _clientHandler.SendRequest( CommandConstants.PublishGame, "");
                            
                            Console.WriteLine(_clientHandler.ReadResponse());

                            break;
                        case "2":

                            _clientHandler.SendRequest( CommandConstants.ListGames, "");

                            Console.WriteLine(_clientHandler.ReadResponse());

                            break;
                        case "3":

                            _clientHandler.SendRequest( CommandConstants.ListGames, "");

                            Console.WriteLine(_clientHandler.ReadResponse());

                            Console.WriteLine("Ingrese el Id del juego que desea ver:");
                            var _gameId = Console.ReadLine();

                            _clientHandler.SendRequest( CommandConstants.ShowGameDetails, _gameId);
                            Console.WriteLine(_clientHandler.ReadResponse());

                            break;
                        default:
                            Console.WriteLine("Opcion invalida");
                            break;
                    }
                }

                catch (Exception ex)
                {
                    _clientHandler.CloseConnection();
                    Console.WriteLine("El servidor cerró la conexión");
                    Console.ReadLine();
                    isRunning = false;
                }
            }
        }

    }
}