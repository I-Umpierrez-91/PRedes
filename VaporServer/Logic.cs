using Common.FileHandler;
using Common.FileHandler.Interfaces;
using DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using VaporServer.Interfaces;

namespace VaporServer
{
    class Logic : ILogic
    {
        static public ICollection<Juego> _juegos = new List<Juego>();
        static public ICollection<Usuario> _usuarios = new List<Usuario>();
        static public ICollection<Review> _review = new List<Review>();
        static public IFileHandler fileHandler = new FileHandler();
        public string PrintGameList() {
            string result = "";
            if (_juegos.Count > 0)
            {
                result = result + ("Listado de todos los juegos: \n");
                foreach (var j in _juegos) {
                    result = result + "Id: " + j.Id.ToString() + " Nombre: " + j.Nombre + " Género: " + j.Genero + "\n";
                    result = result + "Carátula: " + (string.IsNullOrEmpty(j.Caratula) ? "No se agregó carátula" : j.Caratula) + "\n";
                }
                return result;
            }
            else
            {
                return "No hay juegos publicados.";
            }
        }

        public void CreateGame() {
            Console.WriteLine("Creando juego, ingrese los siguientes datos," +
                "Nombre: ");
            var _nombre = Console.ReadLine();
            Console.WriteLine("Genero: ");
            var _genero = Console.ReadLine();
            Console.WriteLine("Sinopsis: ");
            var _sinopsis = Console.ReadLine();
            string path = "No";           
            while (!path.Equals(string.Empty) && !fileHandler.FileExists(path))
            {
                Console.WriteLine("Ingrese un path válido para la carátula (Si no quiere agregar carátula deje el campo vacío): ");
                path = Console.ReadLine();
            }
            var _caratula = path;
            Juego newGame = new Juego(new List<Review>());
            newGame.Nombre = _nombre;
            newGame.Genero = _genero;
            newGame.Sinopsis = _sinopsis;
            newGame.Caratula = _caratula;
            _juegos.Add(newGame);
            Console.WriteLine("Juego creado!");
        }

        public string PrintGameDetails(string idJuego) {
            string result = "";
            var query_1 = from a in _juegos
                            where a.Id.Equals(Int32.Parse(idJuego))
                            select a;
            foreach (var j in query_1)
            {
                //TODO: Agregar excepcion
                result = result + "Detalles del juego: \n";
                result = result + "Id: " + j.Id.ToString() + " Nombre: " + j.Nombre + " Género: " + j.Genero + "\n";
                result = result + "Sinopsis: \n" + j.Sinopsis + "\n";
                result = result + "Reviews:\n";
                if (j.Reviews.Count == 0)
                {
                    result = result + ("No hay reviews.\n");
                }
                else
                {
                    foreach (var r in j.Reviews)
                    {
                        result = result + "Titulo: " + r.Titulo + "\n";
                        result = result + r.Nota.ToString() + "\n";
                    }
                }
            }
            return result;
        }
    }
}
