using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaporServer.Interfaces
{
    public interface ILogic
    {
        public abstract string PrintGameList();

        public abstract string PrintGameDetails(string idJuego);

        public virtual void CreateGame( ) { }
    }
}
