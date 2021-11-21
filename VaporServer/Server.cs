using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Common.FileHandler;
using Common.FileHandler.Interfaces;
using System;
using System.Threading.Tasks;
using VaporServer.Interfaces;

namespace VaporServer
{
    class Server
    {
        static bool _exit = false;
        private static ILogic _logic = new Logic();
        static public IFileHandler _fileHandler = new FileHandler();
        private static async Task Main()
        {
            Console.WriteLine("Iniciando servidor...");
            var _grpdHandler = Task.Run(()=>CreateHostBuilder(null).Build().Run());
            var _LogHandler = new LogHandler();
            Console.WriteLine("Iniciado...");
            //var startLogs = Task.Run(() =>_LogHandler.LogHandlerStart());
            //await _LogHandler.LogHandlerStart();
            var _serverHandler = new ServerHandler(_LogHandler);
            bool testDataLoaded = false;
            var startup = Task.Run(() =>_serverHandler.ServerHandlerStart());
            Console.WriteLine("Esperando a nuevos clientes...");
            Console.WriteLine("Bienvenido al Sistema Server");
            while (!_exit)
            {
                Console.WriteLine("--SERVER--");
                Console.WriteLine("Opciones validas: ");
                Console.WriteLine("0: Salir");
                Console.WriteLine("1: Listar Juegos");
                Console.WriteLine("2: Ver detalle de un Juego");
                Console.WriteLine("3: Publicar un Juego");
                Console.WriteLine("4: Administrar usuarios");
                Console.WriteLine("5: Comprar juego");
                Console.WriteLine("6: Calificar juego");
                Console.WriteLine("7: Buscar juegos");
                Console.WriteLine("8: Ver juegos de usuario");
                Console.WriteLine("9: Opciones del servidor");
                Console.WriteLine("99: Cargar datos de prueba");
                Console.WriteLine("Ingrese el número de su opción: " +
                    "");
                var userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "0":
                        _exit = true;
                        await _serverHandler.CloseConnections();
                        break;
                    case "1":
                        Console.WriteLine(_logic.PrintGameList());
                        Console.WriteLine("Presione enter para continuar...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case "2":
                        Console.WriteLine(_logic.PrintGameList());
                        Console.WriteLine("Ingrese el Id del juego que desea ver:");
                        var _gameId = Console.ReadLine();
                        Console.WriteLine(_logic.PrintGameDetails(_gameId));
                        Console.WriteLine("Presione enter para continuar...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case "3":
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
                        Console.WriteLine(_logic.CreateGame(name, genre, sinopsis, path));
                        Console.WriteLine("Presione enter para continuar...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case "4":
                        Console.WriteLine("1: Alta usuario");
                        Console.WriteLine("2: Modificar password usuario");
                        Console.WriteLine("3: Eliminar usuario");
                        var userInput2 = Console.ReadLine();
                        switch (userInput2)
                        {
                            case "1":
                                Console.WriteLine("Creando usuario, ingrese los siguientes datos, \n" +
                                    "Nombre de usuario: ");
                                var userName = Console.ReadLine();
                                Console.WriteLine("Password: ");
                                var password = Console.ReadLine();
                                Console.WriteLine(_logic.CreateUser(userName, password));
                                break;
                            case "2":
                                Console.WriteLine("Modificando usuario, ingrese los siguientes datos, \n" +
                                    "Nombre de usuario: ");
                                var userNameToModify = Console.ReadLine();
                                Console.WriteLine("Nuevo password: ");
                                var passwordToModify = Console.ReadLine();
                                Console.WriteLine(_logic.ModifyUser(userNameToModify, passwordToModify));
                                break;
                            case "3":
                                Console.WriteLine("Para eliminar un usuario, ingrese el nombre de usuario: ");
                                var userNameToDelete = Console.ReadLine();
                                Console.WriteLine(_logic.DeleteUser(userNameToDelete));
                                break;
                            default:
                                Console.WriteLine("Opcion incorrecta ingresada");
                                break;
                        }
                        Console.WriteLine("Presione enter para continuar...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case "5":
                        Console.WriteLine("Para comprar un juego, ingrese el nombre de usuario \n" +
                            "Nombre: ");
                        var userNameToBuy = Console.ReadLine();
                        Console.WriteLine("Ingrese el id del juego que desea comprar: ");
                        Console.WriteLine(_logic.PrintGameList());
                        var gameIdToBuy = Console.ReadLine();
                        Console.WriteLine(_logic.BuyGame(userNameToBuy, gameIdToBuy));
                        Console.WriteLine("Presione enter para continuar...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case "6":
                        Console.WriteLine("Para calificar un juego, ingrese el nombre de usuario \n" +
                            "Nombre: ");
                        var userNameToReview = Console.ReadLine();
                        Console.WriteLine("Ingrese el id del juego que desea comprar: ");
                        var gameIdToReview = Console.ReadLine();
                        Console.WriteLine("Ingrese una nota del 1 al 5: ");
                        var calification = Console.ReadLine();
                        Console.WriteLine("Ingrese una breve nota para su review: ");
                        var reviewNotes = Console.ReadLine();
                        Console.WriteLine(_logic.ReviewGame(userNameToReview, gameIdToReview, calification, reviewNotes));
                        Console.WriteLine("Presione enter para continuar...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case "7":
                        Console.WriteLine("Para buscar un juego, ingrese los filtros solicitados. \n" +
                            "Si no desea filtrar por un campo, dejelo vacio.\n" +
                            "Nombre: ");
                        var nameFilter = Console.ReadLine();
                        Console.WriteLine("Nota minima: ");
                        var minRatingFilter = Console.ReadLine();
                        Console.WriteLine("Nota maxima: ");
                        var maxRatingFilter = Console.ReadLine();
                        Console.WriteLine("Genero: ");
                        var genreFilter = Console.ReadLine();

                        Console.WriteLine(_logic.GetFilteredGames(nameFilter, minRatingFilter, maxRatingFilter, genreFilter));
                        Console.WriteLine("Presione enter para continuar...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case "8":
                        Console.WriteLine("Para ver los juegos de un usuario, ingrese el nombre de usuario \n" +
                            "Nombre: ");
                        var userNameToGetGames = Console.ReadLine();

                        Console.WriteLine(_logic.GetUserGames(userNameToGetGames));
                        Console.WriteLine("Presione enter para continuar...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case "9":
                        DisplayAdministrationMenu(_LogHandler);
                        break;
                    case "99":
                        if (!testDataLoaded)
                        {
                            TestData.LoadTestData(_logic);
                            testDataLoaded = true;
                            Console.WriteLine("Datos de prueba cargados");
                        }
                        else
                        {
                            Console.WriteLine( "Ya cargaste los datos de test ;)");
                        }
                        Console.WriteLine("Presione enter para continuar...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("Opcion incorrecta ingresada");
                        break;
                }
            }
        }
        private static void DisplayAdministrationMenu(LogHandler _LogHandler)
        {

            var exitAdminMenu = false;
            while (!exitAdminMenu)
            {
                Console.Clear();
                Console.WriteLine("--OPCIONES DE LOGS--");
                Console.WriteLine("Opciones validas: ");
                Console.WriteLine("0: Salir");
                Console.WriteLine("1: Logs solo en consola");
                Console.WriteLine("2: Logs en consola y a Rabbit MQ");
                Console.WriteLine("3: Logs solo a Rabbit MQ");
                Console.WriteLine("Ingrese el número de su opción: " +
                    "");
                var userInput = Console.ReadLine();
                switch (userInput)
                {                    
                    case "0":
                        exitAdminMenu = true;
                        Console.Clear();
                        break;
                    case "1":
                        _LogHandler.SetLogOption(1);
                        break;
                    case "2":
                        _LogHandler.SetLogOption(2);
                        break;
                    case "3":
                        _LogHandler.SetLogOption(3);
                        break;
                    default:
                        Console.WriteLine("Opcion incorrecta ingresada");
                    break;
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(5001, o => o.Protocols = HttpProtocols.Http2);
            });
            webBuilder.UseStartup<Startup>();
        });
    }
}

