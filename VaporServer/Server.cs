
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
            var _serverHandler = new ServerHandler();
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
                        break;
                    case "2":
                        Console.WriteLine(_logic.PrintGameList());
                        Console.WriteLine("Ingrese el Id del juego que desea ver:");
                        var _gameId = Console.ReadLine();
                        Console.WriteLine(_logic.PrintGameDetails(_gameId));
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
                            path = Console.ReadLine();
                        }
                        Console.WriteLine(_logic.CreateGame(name, genre, sinopsis, path));
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
                        break;
                    case "5":
                        Console.WriteLine("Para comprar un juego, ingrese el nombre de usuario \n" +
                            "Nombre: ");
                        var userNameToBuy = Console.ReadLine();
                        Console.WriteLine("Ingrese el id del juego que desea comprar: ");
                        var gameIdToBuy = Console.ReadLine();
                        Console.WriteLine(_logic.BuyGame(userNameToBuy, gameIdToBuy));

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

                        break;
                    default:
                        Console.WriteLine("Opcion incorrecta ingresada");
                        break;
                }
            }
        }
    }
}

