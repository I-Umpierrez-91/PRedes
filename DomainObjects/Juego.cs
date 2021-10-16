using System;
using System.Collections.Generic;
using System.Threading;

namespace DomainObjects
{
    public class Juego
    {
        static int nextId;
        public int Id { get; private set; }
        public string Nombre { get; set; }
        public string Genero { get; set; }
        public float Rating { get; set; }
        public string Sinopsis { get; set; }
        //caratula.
        public ICollection<Review> Reviews { get; set; }
        public Juego(ICollection<Review> reviews)
        {
            Reviews = reviews;
            Rating = 0;
            Id = Interlocked.Increment(ref nextId);
        }
    }
}
