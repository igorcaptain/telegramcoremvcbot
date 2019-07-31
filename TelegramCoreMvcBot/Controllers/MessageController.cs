using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramCoreMvcBot.Models;

namespace TelegramCoreMvcBot.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/message/update")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        /*
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }
        */
        [HttpPost]
        public async Task<OkResult> Post([FromBody]Update update)
        {
            // var exceptList = new[] { "~", "`", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "+", "=", "\"", "_" };
            // || update.Message.Text == "" || exceptList.Any(update.Message.Text.Contains)
            if (update == null) return Ok();
            
            var commands = Bot.Comands;
            var message = update.Message;
            var botClient = await Bot.GetBotClientAsync();

            foreach(var command in commands)
            {
                if(command.Contains(message))
                {
                    await command.Execute(message, botClient);
                    break;
                }
            }
            return Ok();
        }

    }
}
