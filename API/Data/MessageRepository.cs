using API.DTOs;
using API.Entites;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.Execution;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext dataContext,IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public  void AddMessage(Message message)
        {
           _dataContext.Message.AddAsync(message);
        }

        public void DeleteMessage(Message message)
        {
            _dataContext.Message.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
           return await _dataContext.Message.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser([FromQuery] MessageParams messageParams)
        {
            var query = _dataContext.Message
                .OrderByDescending(x=>x.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch {
                "Inbox" => query.Where(x => x.RecipientUserName == messageParams.UserName && x.RecipientDeleted == false),
                "Outbox" =>query.Where(x=>x.SenderUserName == messageParams.UserName && x.SenderDeleted == false),
                _=>query.Where(x=>x.RecipientUserName == messageParams.UserName && x.DateRead ==null)
            };

            return await PagedList<MessageDto>.CreateAsync(query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).
                AsNoTracking(), messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipiantUserName)
        {
            var messages =await _dataContext.Message
                           .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                           .Include(x => x.Sender).ThenInclude(x => x.Photos)
                           .Where(x => x.RecipientUserName == currentUserName && x.SenderUserName == recipiantUserName && x.RecipientDeleted == false
                           || x.RecipientUserName == recipiantUserName && x.SenderUserName == currentUserName && x.SenderDeleted == false)
                           .OrderBy(x => x.MessageSent)
                           .ToListAsync();
            var unreadMessages = messages.Where(x => x.DateRead == null && x.RecipientUserName == currentUserName);
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.Now;

                await _dataContext.SaveChangesAsync();
            }
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
