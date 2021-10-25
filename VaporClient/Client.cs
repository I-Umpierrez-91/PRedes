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
            try
            {
                await _clientHandler.ClientHandlerStart();
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo conectar al servidor, verifique que el servidor este corriendo. ");
                Console.ReadLine();
                isRunning = false;
            }

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
                    if (responseText.Equals("200"))
                    {
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
                    Console.WriteLine("5 -> Calificar juego");
                    Console.WriteLine("6 -> Buscar juegos");
                    Console.WriteLine("7 -> Ver mis juegos");
                    Console.WriteLine("Ingrese su opcion: ");
                    var option = Console.ReadLine();
                    switch (option)
                    {
                        case "0":
                            await _clientHandler.CloseConnection();
                            isRunning = false;
                            break;
                        case "1":

                            Console.WriteLine("Ingrese el Id del juego que desea Comprar:");
                            await _clientHandler.SendRequest(CommandConstants.ListGames, "");
                            Console.WriteLine(await _clientHandler.ReadResponse());
                            var gameIdToPurchase = Console.ReadLine();

                            await _clientHandler.SendRequest(CommandConstants.PurchaseGame, _username + div + gameIdToPurchase);
                            Console.WriteLine(await _clientHandler.ReadResponse());

                            Console.WriteLine("Presione enter para continuar...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        case "2":

                            await _clientHandler.SendRequest(CommandConstants.ListGames, "");
                            var responseText = await _clientHandler.ReadResponse();
                            Console.WriteLine(responseText);
                            Console.WriteLine("Presione enter para continuar...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        case "3":

                            await _clientHandler.SendRequest(CommandConstants.ListGames, "");

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
                            Console.WriteLine("Presione enter para continuar...");
                            Console.ReadLine();
                            Console.Clear();
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
                                path = Console.ReadLine().Replace("\"", "");
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
                            Console.WriteLine("Presione enter para continuar...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        case "5":
                            await _clientHandler.SendRequest(CommandConstants.ListGames, "");
                            Console.WriteLine(await _clientHandler.ReadResponse());

                            Console.WriteLine("Ingrese el Id del juego que desea calificar:");
                            var gameIdToReivew = Console.ReadLine();
                            Console.WriteLine("Ingrese una nota del 1 al 5:");
                            var calification = Console.ReadLine();
                            Console.WriteLine("Ingrese una breve nota para su review:");
                            var reviewNotes = Console.ReadLine();
                            var reviewRequestMessage = _username + div + gameIdToReivew + div + calification + div + reviewNotes;

                            await _clientHandler.SendRequest(CommandConstants.PublishReview, reviewRequestMessage);
                            Console.WriteLine(await _clientHandler.ReadResponse());
                            Console.WriteLine("Presione enter para continuar...");
                            Console.ReadLine();
                            Console.Clear();

                            break;
                        case "6":
                            Console.WriteLine("Ingrese los filtros solicitados. \n" +
                                "Si no desea filtrar por un campo, dejelo vacio. \n" +
                                "Nombre: ");
                            var nameFilter = Console.ReadLine();
                            Console.WriteLine("Nota minima: ");
                            var minRatingFilter = Console.ReadLine();
                            Console.WriteLine("Nota maxima: ");
                            var maxRatingFilter = Console.ReadLine();
                            Console.WriteLine("Genero: ");
                            var genreFilter = Console.ReadLine();

                            var filterGamesMessage = nameFilter + div + minRatingFilter + div + maxRatingFilter + div + genreFilter;

                            await _clientHandler.SendRequest(CommandConstants.FilterGames, filterGamesMessage);
                            Console.WriteLine(await _clientHandler.ReadResponse());
                            Console.WriteLine("Presione enter para continuar...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        case "7":
                            await _clientHandler.SendRequest(CommandConstants.GetUserGames, _username);
                            Console.WriteLine(await _clientHandler.ReadResponse());
                            Console.WriteLine("Presione enter para continuar...");
                            Console.ReadLine();
                            Console.Clear();
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