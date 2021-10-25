﻿using System;
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

        public abstract string CreateGame(string name, string genre, string sinopsis, string path);

        public abstract string GetGamePhotoPath(string idJuego);

        public abstract string CreateUser(string username, string password);

        public abstract string ModifyUser(string username, string password);

        public abstract string DeleteUser(string username);
    }
}
