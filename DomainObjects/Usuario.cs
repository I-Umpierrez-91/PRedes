using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects
{
    public class Usuario
    {
        public int Id { get; set; }
        public ICollection<Juego> Juegos { get; set; }
        public Usuario(ICollection<Juego> juegos)
        {
            Juegos = juegos;
        }
    }
}
