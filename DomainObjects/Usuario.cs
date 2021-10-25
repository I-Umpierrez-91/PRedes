using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DomainObjects
{
    public class Usuario
    {
        static int nextId;
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public ICollection<Juego> Juegos { get; set; } = new List<Juego>();
        public Usuario()
        {
            Id = Interlocked.Increment(ref nextId);
        }
    }
}
