using Grpc.Core;
using System;
using System.Threading.Tasks;
using VaporServer.Interfaces;

namespace VaporServer.Services
{
    public class UserGRPCService : UserService.UserServiceBase
    {
        private readonly ILogic _logic = new Logic();
        public override Task<StringResult> CreateUser(UserRequest request, ServerCallContext context)
        {
            var retMessage = "";
            try
            {
                retMessage = _logic.CreateUser(request.Username, request.Password);
            }
            catch (Exception e)
            {

                retMessage = e.Message;
            }

            return Task.FromResult(new StringResult {

                Message = retMessage

            });
        }

        public override Task<StringResult> ModifyUser(UserRequest request, ServerCallContext context)
        {
            var retMessage = "";
            try
            {
                retMessage = _logic.ModifyUser(request.Username, request.Password);
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
        public override Task<StringResult> DeleteUser(UserRequest request, ServerCallContext context)
        {
            var retMessage = "";
            try
            {
                retMessage = _logic.DeleteUser(request.Username);
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
    }
}
