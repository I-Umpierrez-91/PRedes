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
            if (_juegos.Count > 0)
            {
                return PrintGameList(_juegos);
            }
            else
            {
                return "No hay juegos publicados.";
            }
        }

        public string PrintGameList(IEnumerable<Juego> games)
        {
            string result = "";
            result = result + ("Listado de juegos: \n");
            foreach (var j in _juegos)
            {
                result = result + "Id: " + j.Id.ToString() + " Nombre: " + j.Nombre + " Género: " + j.Genero + "\n";
                result = result + "Carátula: " + (string.IsNullOrEmpty(j.Caratula) ? "No se agregó carátula" : j.Caratula) + "\n";
            }
            return result;
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

        public string ReviewGame(string username, string gameId, string calification, string reviewNotes)
        {
            try
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
                        Review newReview = new Review()
                        {
                            Nota = Int32.Parse(calification),
                            Titulo = reviewNotes
                        };
                        queryGameDetails.FirstOrDefault().Reviews.Add(newReview);
                        var reviews = queryGameDetails.FirstOrDefault().Reviews;
                        float average = 0;
                        foreach (var r in reviews)
                        {
                            average = average + r.Nota;
                        }
                        queryGameDetails.FirstOrDefault().Rating = (average / queryGameDetails.FirstOrDefault().Reviews.Count());

                        return ((Review.minNota <= Int32.Parse(calification)) && (Int32.Parse(calification)) <= Review.maxNota ? "" : "La nota se limitó al máximo/mínimo. ") + "El review se agregó correctamente.";
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
            catch (Exception ex)
            {
                return "Ocurrió un problema al procesar la solicitud.";
            }
        }

        public string GetFilteredGames(string nameFilter, string minRatingFilter, string maxRatingFilter, string genreFilter)
        {
            var maxRating = (maxRatingFilter.Equals(string.Empty) ? Review.maxNota : int.Parse(maxRatingFilter));
            var minRating = (minRatingFilter.Equals(string.Empty) ? Review.minNota : int.Parse(minRatingFilter));
            var filteredGames = _juegos.Where(j => j.Rating <= maxRating && j.Rating >= minRating);
            if (!nameFilter.Equals(string.Empty))
            {
                filteredGames = filteredGames.Where(j => j.Nombre.Equals(nameFilter));
            }
            if (!genreFilter.Equals(string.Empty))
            {
                filteredGames = filteredGames.Where(j => j.Genero.Equals(genreFilter));
            }
            if (filteredGames.Count() > 0)
            {
                return PrintGameList(filteredGames);
            }
            else
            {
                return "No hay juegos que cumplan con las condiciones.";
            }
        }
    }
}
