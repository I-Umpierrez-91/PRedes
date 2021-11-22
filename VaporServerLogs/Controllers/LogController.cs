using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VaporServerLogs.Controllers
{
    [ApiController]
    [Route("/Logs")]
    public class LogController : ControllerBase
    {
        private LogLogic _logic;

        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger, LogLogic logLogic)
        {
            _logger = logger;
            _logic = logLogic;
        }

        [HttpGet]
        //logs?gameId=3&username=juancito
        public IActionResult Get()
        {
            string username = HttpContext.Request.Query["username"].ToString();
            string gameId = HttpContext.Request.Query["gameId"].ToString();
            string date = HttpContext.Request.Query["date"].ToString();

            return new OkObjectResult(_logic.ReadLogs(username, gameId, date));
        }
    }
}
