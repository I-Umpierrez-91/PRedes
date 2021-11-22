using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VaporAdministrativeServer.Controllers
{
    [ApiController]
    [Route("/Users")]
    public class UserController : ControllerBase
    {
        [HttpPost]
        //User
        public async Task<IActionResult> PostAsync([FromBody] UserDTOForPost userData)
        {
            var response = await Logic.CreateUser(userData);
            return new OkObjectResult(response);
        }

        [HttpPut]
        //User
        public async Task<IActionResult> UpdateUser([FromBody] UserDTOForPost userData)
        {
            var response = await Logic.ModifyUser(userData);
            return new OkObjectResult(response);
        }

        [HttpDelete]
        //User
        public async Task<IActionResult> DeleteUser([FromBody] UserDTOForPost userData)
        {
            var response = await Logic.DeleteUser(userData);
            return new OkObjectResult(response);
        }
    }
}
