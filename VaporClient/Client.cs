using Common;
using Common.FileHandler;
using Common.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using ProtocolLibrary;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VaporClient
{
    class Client
    {
        static readonly ISettingsManager SettingsMgr = new SettingsManager();

        static async Task Main(string[] args)
        {
            var isRunning = true;
            var isLogged = false;
            var _username = "";
            var _clientHandler = new ClientHandler();
            var _fileHandler = new FileHandler();
            var div = HeaderConstants.Divider;

            await _clientHandler.ClientHandlerStart();

            while (isRunning && !isLogged)
            {
                try
                {
                    Console.WriteLine("--CLIENTE--");
                    Console.WriteLine("Inicio de sesion: ");
                    Console.WriteLine("Nombre de usuario: ");
                    var name = Console.ReadLine();
                    Console.WriteLine("Password: ");
                    var password = Console.ReadLine();

                    div = HeaderConstants.Divider;
                    var requestMessage = name + div + password;
                    await _clientHandler.SendRequest(CommandConstants.Login, requestMessage);

                    var responseText = await _clientHandler.ReadResponse();
                    if (responseText.Equals("200")) {
                        _username = name;
                        isLogged = true;
                    }
                    else
                    {
                        Console.WriteLine("Nombre de usuario o contraseña incorrectos.");
                    }

                }
                catch (SocketException ex)
                {
                    await _clientHandler.CloseConnection();
                    Console.WriteLine("El servidor cerró la conexión");
                    Console.ReadLine();
                    isRunning = false;
                }
                catch (Exception ex)
                {
                    await _clientHandler.CloseConnection();

                    Console.WriteLine("Error interno, cerrando conexion " + ex.ToString());
                    Console.ReadLine();
                    isRunning = false;
                }
            }
                    while (isRunning && isLogged)
            {
                try
                {
                    
                    Console.WriteLine("--CLIENTE--");
                    Console.WriteLine("Opciones validas: ");
                    Console.WriteLine("0 -> Salir");
                    Console.WriteLine("1 -> Comprar juego");
                    Console.WriteLine("2 -> Listar juegos");
                    Console.WriteLine("3 -> Detalle juego");
                    Console.WriteLine("4 -> Publicar juego");
                    Console.WriteLine("Ingrese su opcion: ");
                    var option = Console.ReadLine();
                    switch (option)
                    {
                        case "0":
                            await _clientHandler.CloseConnection();
                            isRunning = false;
                            break;
                        case "1":

                            await _clientHandler.SendRequest(CommandConstants.ListGames, "");
                            Console.WriteLine(await _clientHandler.ReadResponse());

                            Console.WriteLine("Ingrese el Id del juego que desea Comprar:");
                            var gameIdToPurchase = Console.ReadLine();

                            await _clientHandler.SendRequest(CommandConstants.PurchaseGame, _username + div + gameIdToPurchase);
                            Console.WriteLine(await _clientHandler.ReadResponse());

                            break;
                        case "2":

                            await _clientHandler.SendRequest( CommandConstants.ListGames, "");
                            var responseText = await _clientHandler.ReadResponse();
                            Console.WriteLine(responseText);

                            break;
                        case "3":

                            await _clientHandler.SendRequest( CommandConstants.ListGames, "");

                            Console.WriteLine(await _clientHandler.ReadResponse());

                            Console.WriteLine("Ingrese el Id del juego que desea ver:");
                            var gameId = Console.ReadLine();

                            await _clientHandler.SendRequest(CommandConstants.ShowGameDetails, gameId);
                            Console.WriteLine(await _clientHandler.ReadResponse());

                            var opt = "";
                            while (!opt.Equals("Si") && !opt.Equals("No"))
                            {
                                Console.WriteLine("Desea descargar la carátula? (Si/No):");
                                opt = Console.ReadLine();
                            }

                            if (opt.Equals("Si"))
                            {
                                await _clientHandler.SendRequest(CommandConstants.RequestGamePhoto, gameId);
                                string workingDirectory = Environment.CurrentDirectory;
                                Console.WriteLine(await _clientHandler.ReceiveFile());
                            }

                            break;
                        case "4":

                            Console.WriteLine("Creando juego, ingrese los siguientes datos," +
                                "Nombre: ");
                            var name = Console.ReadLine();
                            Console.WriteLine("Genero: ");
                            var genre = Console.ReadLine();
                            Console.WriteLine("Sinopsis: ");
                            var sinopsis = Console.ReadLine();
                            string path = "No";
                            while (!path.Equals(string.Empty) && !_fileHandler.FileExists(path))
                            {
                                Console.WriteLine("Ingrese un path válido para la carátula (Si no quiere agregar carátula deje el campo vacío): ");
                                path = Console.ReadLine();
                            }
                            
                            var requestMessage = name + div + genre + div + sinopsis + div + 
                                (path.Equals(string.Empty) ? "" : _fileHandler.GetFileName(path)) + div +
                                (path.Equals(string.Empty) ? "" : _fileHandler.GetFileSize(path).ToString());

                            await _clientHandler.SendRequest(CommandConstants.PublishGame, requestMessage);

                            if (!path.Equals(string.Empty))
                            {                                
                                await _clientHandler.SendParts(path);
                            }

                            Console.WriteLine(await _clientHandler.ReadResponse());

                            break;
                        default:
                            Console.WriteLine("Opcion invalida");
                            break;
                    }
                }

                catch (SocketException ex)
                {
                    await _clientHandler.CloseConnection();
                    Console.WriteLine("El servidor cerró la conexión");
                    Console.ReadLine();
                    isRunning = false;
                }
                catch (Exception ex)
                {
                    await _clientHandler.CloseConnection();

                    Console.WriteLine("Error interno, cerrando conexion " + ex.ToString());
                    Console.ReadLine();
                    isRunning = false;
                }
            }
        }

    }
}