using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace VaporServer
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
        
        public override Task<NumberResponse> GiveMeANumber(NumberRequest request, ServerCallContext context)
        {
            return Task.FromResult(new NumberResponse {NumberResult = RandomNumber(request.NumberParameter)});
        }

        public override Task<UserResponseList> GiveMeUsers(UserRequest request, ServerCallContext context)
        {
            return Task.FromResult(CreateUserList());
        }

        private static UserResponseList CreateUserList()
        {
            var list = new UserResponseList();
            var user1 = new User {UserAge = 10, UserName = "Felipe"};
            var user2 = new User {UserAge = 25, UserName = "Victoria"};
            list.Users.Add(user1);
            list.Users.Add(user2);
            return list;
        }

        private static int RandomNumber(int numberBase)
        {
            var rand = new Random();
            return rand.Next(1, 1000) * numberBase;
        }

    }
}