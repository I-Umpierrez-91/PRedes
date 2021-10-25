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

        public string CreateGame(string name, string genre, string sinopsis, string path) {

            Juego newGame = new Juego(new List<Review>());
            newGame.Nombre = name;
            newGame.Genero = genre;
            newGame.Sinopsis = sinopsis;
            newGame.Caratula = path;
            try
            {
                _juegos.Add(newGame);
            }
            catch (Exception ex)
            {
                return "Error al crear el juego.";
            }
            return "Juego creado!";
        }

        public string PrintGameDetails(string gameId) {
            string result = "";
            var queryGameDetails = from a in _juegos
                            where a.Id.Equals(Int32.Parse(gameId))
                            select a;
            foreach (var j in queryGameDetails)
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

        public string GetGamePhotoPath(string gameId)
        {
            var querySelectedGame = from a in _juegos
                          where a.Id.Equals(Int32.Parse(gameId))
                          select a;
            return querySelectedGame.FirstOrDefault().Caratula;
        }

        public string CreateUser(string username, string password)
        {
            var querySelectedUser = from a in _usuarios
                                    where a.UserName.Equals(username)
                                    select a;
            if (querySelectedUser.Count() > 0)
            {
                return "nomre de usuario ya existe. Elija otro";
            }
            else
            {
                Usuario user = new Usuario()
                {
                    UserName = username,
                    Password = password
                };
                _usuarios.Add(user);
                return "usuario creado";
            }
        }

        public string ModifyUser(string username, string password)
        {
            var querySelectedUser = from a in _usuarios
                                    where a.UserName.Equals(username)
                                    select a;
            if (querySelectedUser.Count()>0)
            {
                querySelectedUser.FirstOrDefault().Password = password;
                return "Password actualizado.";
            }
            else
            {
                return "Username incorrecto.";
            }

        }

        public string DeleteUser(string username)
        {
            var querySelectedUser = from a in _usuarios
                                    where a.UserName.Equals(username)
                                    select a;
            if (querySelectedUser.Count() > 0)
            {
                _usuarios.Remove(querySelectedUser.FirstOrDefault());
                return "Usuario eliminado";
            }
            else
            {
                return "Usuario no encontrado";
            }
        }

        public bool Login(string username, string password) {
            var querySelectedUser = from a in _usuarios
                                    where a.UserName.Equals(username)
                                    select a;
            if (querySelectedUser.Count() > 0)
            {
                return querySelectedUser.FirstOrDefault().Password.Equals(password);
            }
            else
            {
                return false;
            }
        }

        public string BuyGame(string username, string gameId)
        {
            var querySelectedUser = from a in _usuarios
                                    where a.UserName.Equals(username)
                                    select a;
            if (querySelectedUser.Count() > 0)
            {
                var queryGameDetails = from a in _juegos
                                       where a.Id.Equals(Int32.Parse(gameId))
                                       select a;
                if (queryGameDetails.Count() > 0)
                {
                    querySelectedUser.FirstOrDefault().Juegos.Add(queryGameDetails.FirstOrDefault());
                    return "Juego agregado al usuario";
                }
                else
                {
                    return "El juego no existe";
                } 
            }
            else
            {
                return "El usuario no existe";
            }
        }
    }
}
