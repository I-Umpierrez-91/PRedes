using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VaporAdministrativeServer.Controllers
{
    [ApiController]
    [Route("/Games")]
    public class GameController : ControllerBase
    {

        [HttpPost]
        //game
        public async Task<IActionResult> PostAsync([FromBody] GameDTOForPost gameData)
        {
            var response = await Logic.CreateGame(gameData);
            return new OkObjectResult(response);
        }

        [HttpPost("Buy")]
        //game
        public async Task<IActionResult> AssignToUserAsync([FromBody] GameBuyDTOForPost gameBuyData)
        {
            var response = await Logic.BuyGame(gameBuyData);
            return new OkObjectResult(response);
        }

    }
}
