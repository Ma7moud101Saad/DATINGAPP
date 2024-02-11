using API.Data;
using API.DTOs;
using API.Entites;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    [Authorize]
    public class MessageHub:Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;

        public MessageHub(IMessageRepository messageRepository,
            IUserRepository userRepository,
            IMapper mapper,
            IHubContext<PresenceHub> presenceHub)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _presenceHub = presenceHub;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext=Context.GetHttpContext();
            var otherUser=httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUserName(),otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var group= await AddToGroup(groupName);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages=await _messageRepository.GetMessageThread(Context.User.GetUserName(),otherUser);

            await Clients.Caller.SendAsync("ReceveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group=await RmoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var userName = Context.User.GetUserName();

            if (userName == createMessageDto.RecipientUserName.ToLower())
                throw new HubException("You can`t send message to your self");

            var sender = await _userRepository.GetUserByNameAsync(userName);
            var recipiant = await _userRepository.GetUserByNameAsync(createMessageDto.RecipientUserName);

            if (recipiant == null) throw new HubException("Not found user");

            Message message = new Message
            {
                SenderUserName = sender.UserName,
                RecipientUserName = recipiant.UserName,
                Sender = sender,
                Recipient = recipiant,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recipiant.UserName);

            var group=await _messageRepository.GetMessageGroup(groupName);

            if (group.connections.Any(x => x.UserName == recipiant.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else {
                var connections = await PresenceTracker.GetConnectionForUser(recipiant.UserName);
                if (connections!= null) {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new { 
                        userName = sender.UserName,
                        knownAs=sender.KnownAs
                    });
                }
            }

            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }
        private string GetGroupName(string caller,string other) { 
            var stringCompare=string.CompareOrdinal(caller,other) < 0;
            return stringCompare ? $"{caller}-{other}":$"{ other }-{caller}";
        }

        private async Task<Group> AddToGroup(string groupName) {
          var group = await _messageRepository.GetMessageGroup(groupName);
          var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

            if (group == null)
            {
                group = new Group(groupName);
                group.connections.Add(connection);
                _messageRepository.AddGroup(group);
            }

            group.connections.Add(connection);
            if(await _messageRepository.SaveAllAsync()) return group;

            throw new HubException();
        }

        private async Task<Group> RmoveFromMessageGroup() {

            var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);

            var connection= group.connections.FirstOrDefault(c=>c.ConnectionId == Context.ConnectionId);

            _messageRepository.RemoveConnection(connection);

             if(await _messageRepository.SaveAllAsync()) return group;

             throw new HubException("Faild to remove from group");
        }
    }
}
