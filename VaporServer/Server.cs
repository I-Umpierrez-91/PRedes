
using System;
using VaporServer.Interfaces;

namespace VaporServer
{
    class Server
    {
        static bool _exit = false;
        private static ILogic _logic = new Logic();
        private static void Main()
        {
            Console.WriteLine("Iniciando servidor...");
            var _serverHandler = new ServerHandler();
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
                        _serverHandler.CloseConnections();
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
                        _logic.CreateGame();
                        break;
                    default:
                        Console.WriteLine("Opcion incorrecta ingresada");
                        break;
                }
            }
        }
    }
}

