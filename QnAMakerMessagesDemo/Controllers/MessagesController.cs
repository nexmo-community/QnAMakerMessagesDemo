using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nexmo.Api;
using System.Net;

namespace QnAMakerMessagesDemo.Controllers
{
    public class MessagesController : Controller
    {
        private IConfiguration _config;

        public MessagesController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public HttpStatusCode Status()
        {
            return HttpStatusCode.NoContent;
        }

        [HttpGet]
        public HttpStatusCode InboundSms([FromQuery] SMS.SMSInbound inboundMessage)
        {
            _ = Questioner.AskQuestion(inboundMessage.msisdn, inboundMessage.to, "sms", inboundMessage.text, _config);
            return HttpStatusCode.NoContent;
        }

        [HttpPost]
        public HttpStatusCode Inbound([FromBody]InboundMessage message)
        {
            if (message.from.type == "messenger")
            {
                _ = Questioner.AskQuestion(message.from.id, message.to.id, message.from.type, message.message.content.text, _config);
            }
            else
            {
                _ = Questioner.AskQuestion(message.from.number, message.to.number, message.from.type, message.message.content.text, _config);
            }
            return HttpStatusCode.NoContent;
        }
    }
}