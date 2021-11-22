using Grpc.Core;
using System;
using System.Threading.Tasks;
using VaporServer.Interfaces;

namespace VaporServer.Services
{
    public class GameGRPCService : GameService.GameServiceBase
    {
        private readonly ILogic _logic = new Logic();
        public override Task<StringResult> CreateGame(GameRequest request, ServerCallContext context)
        {
            var retMessage = "";
            try
            {
                retMessage = _logic.CreateGame(request.Name, request.Genre, request.Sinopsis, request.Path);
            }
            catch (Exception e)
            {

                retMessage = e.Message;
            }

            return Task.FromResult(new StringResult
            {

                Message = retMessage

            });
        }

        public override Task<StringResult> BuyGame(GameBuyRequest request, ServerCallContext context)
        {
            var retMessage = "";
            try
            {
                retMessage = _logic.BuyGame(request.Username, request.GameId);
            }
            catch (Exception e)
            {

                retMessage = e.Message;
            }

            return Task.FromResult(new StringResult
            {

                Message = retMessage

            });
        }

        public override Task<StringResult> ModifyGame(GameRequest game, ServerCallContext context)
        {
            var retMessage = "not implemented";
            return Task.FromResult(new StringResult
            {

                Message = retMessage

            });
        }

        public override Task<StringResult> DeleteGame(GameRequest game, ServerCallContext context)
        {
            var retMessage = "not implemented";
            return Task.FromResult(new StringResult
            {

                Message = retMessage

            });
        }

        public override Task<StringResult> ReturnGame(GameBuyRequest game, ServerCallContext context)
        {
            var retMessage = "not implemented";
            return Task.FromResult(new StringResult
            {

                Message = retMessage

            });
        }

    }
}
