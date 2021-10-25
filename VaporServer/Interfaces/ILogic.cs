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

        public abstract string PrintGameDetails(string gameId);

        public abstract string CreateGame(string name, string genre, string sinopsis, string path);

        public abstract string GetGamePhotoPath(string idJuego);

        public abstract string CreateUser(string username, string password);

        public abstract string ModifyUser(string username, string password);

        public abstract string DeleteUser(string username);
        public abstract bool Login(string username, string password);
        public abstract string BuyGame(string username, string gameId);
        public abstract string ReviewGame(string username, string gameId, string calification, string reviewNotes);
    }
}
