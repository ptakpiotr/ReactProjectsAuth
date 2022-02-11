using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReactProjectsAuthApi.Data;
using ReactProjectsAuthApi.Models;

namespace ReactProjectsAuthApi.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly ChatDbContext _ctx;
        private readonly IMapper _mapper;

        public MessageController(ILogger<MessageController> logger,ChatDbContext ctx,IMapper mapper)
        {
            _logger = logger;
            _ctx = ctx;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult GetAllMessageForConversation(RequestMessagesModel md)
        {
            var msgs = _ctx.Messages.Where(m => m.From == md.From && m.To == md.To).ToList();

            if(msgs is null)
            {
                return NotFound();
            }
            msgs = msgs.OrderBy(m => m.TimeOfCreation).ToList();
            var output = _mapper.Map<List<ChatMessageModel>>(msgs);

            return (Ok(output));
        }
    }
}
