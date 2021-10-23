using Common;
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
    class Client
    {
        static readonly ISettingsManager SettingsMgr = new SettingsManager();

        static async Task Main(string[] args)
        {
            var isRunning = true;
            var _clientHandler = new ClientHandler();
            await _clientHandler.ClientHandlerStart();

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
                            await _clientHandler.CloseConnection();
                            isRunning = false;
                            break;
                        case "1":

                            await _clientHandler.SendRequest( CommandConstants.PublishGame, "");
                            
                            Console.WriteLine(_clientHandler.ReadResponse());

                            break;
                        case "2":

                            await _clientHandler.SendRequest( CommandConstants.ListGames, "");
                            var responseText = await _clientHandler.ReadResponse();

                            Console.WriteLine(responseText);

                            break;
                        case "3":

                            await _clientHandler.SendRequest( CommandConstants.ListGames, "");

                            Console.WriteLine(_clientHandler.ReadResponse());

                            Console.WriteLine("Ingrese el Id del juego que desea ver:");
                            var _gameId = Console.ReadLine();

                            await _clientHandler.SendRequest( CommandConstants.ShowGameDetails, _gameId);
                            Console.WriteLine(_clientHandler.ReadResponse());

                            break;
                        default:
                            Console.WriteLine("Opcion invalida");
                            break;
                    }
                }

                catch (Exception ex)
                {
                    await _clientHandler.CloseConnection();
                    Console.WriteLine("El servidor cerró la conexión");
                    Console.ReadLine();
                    isRunning = false;
                }
            }
        }

    }
}