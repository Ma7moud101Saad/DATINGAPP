using API.DTOs;
using API.Entites;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public MessagesController(
            IMessageRepository messageRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {

            var userName = User.GetUserName();

            if (userName == createMessageDto.RecipientUserName.ToLower()) return BadRequest("You can`t send message to your self");

            var sender = await _userRepository.GetUserByNameAsync(userName);
            var recipiant = await _userRepository.GetUserByNameAsync(createMessageDto.RecipientUserName);

            if(recipiant == null) { return NotFound(); }

            Message message = new Message
            {
                SenderUserName = sender.UserName,
                RecipientUserName=recipiant.UserName,
                Sender=sender,
                Recipient=recipiant,
                Content=createMessageDto.Content
            };
            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("Faild to send message");
        }
        [HttpGet]
        public async Task<IActionResult> GetMessageForUser([FromQuery] MessageParams messageParams) {
            messageParams.UserName=User.GetUserName();
            var messages= await _messageRepository.GetMessageForUser(messageParams);
            Response.AddPaginationHeader(new PaginationHeader(messages.PageNumber, messages.PageSize, messages.TotalCount, messages.TotalPages));
            return Ok(messages);
        }
        [HttpGet("thread/{userName}")]
        public async Task<IEnumerable<MessageDto>> GetMessageThread(string userName) { 
            string currentUserName=User.GetUserName();
           return await _messageRepository.GetMessageThread(currentUserName,userName);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id) { 
            var userName=User.GetUserName();
            var message=await _messageRepository.GetMessage(id);
            if(message.SenderUserName != userName && message.RecipientUserName != userName) return Unauthorized();
            if (message.SenderUserName == userName) message.SenderDeleted = true;
            if (message.RecipientUserName == userName) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
                 _messageRepository.DeleteMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}
