using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaporAdministrativeServer.Controllers;

namespace VaporAdministrativeServer
{
    public static class Logic
    {
        private static UserService.UserServiceClient _userClient;

        private static GameService.GameServiceClient _gameClient;
        public static void initializeLogic(UserService.UserServiceClient userClient, GameService.GameServiceClient gameClient)
        {
            _gameClient = gameClient;

            _userClient = userClient;

        }

        public static async Task<string> CreateGame(GameDTOForPost game)
        {
            var response = await _gameClient.CreateGameAsync(new GameRequest
            {
                Name = game.Name,
                Genre = game.Genre,
                Path = game.Path,
                Sinopsis = game.Sinopsis
            });
            return response.Message;
        }

        public static async Task<string> BuyGame(GameBuyDTOForPost game)
        {
            var response = await _gameClient.BuyGameAsync(new GameBuyRequest
            {
                Username = game.UserName,
                GameId = game.GameId
            });
            return response.Message;
        }

        public static async Task<string> CreateUser(UserDTOForPost user)
        {
            var response = await _userClient.CreateUserAsync(new UserRequest
            {
                Username = user.UserName,
                Password = user.Password
            });
            return response.Message;
        }

        public static async Task<string> ModifyUser(UserDTOForPost user)
        {
            var response = await _userClient.ModifyUserAsync(new UserRequest
            {
                Username = user.UserName,
                Password = user.Password
            });
            return response.Message;
        }

        public static async Task<string> DeleteUser(UserDTOForPost user)
        {
            var response = await _userClient.DeleteUserAsync(new UserRequest
            {
                Username = user.UserName,
                Password = user.Password
            });
            return response.Message;
        }
    }
}
