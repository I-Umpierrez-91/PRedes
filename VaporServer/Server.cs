
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
                    default:
                        Console.WriteLine("Opcion incorrecta ingresada");
                        break;
                }
            }
        }
    }
}

