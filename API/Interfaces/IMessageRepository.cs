using API.DTOs;
using API.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entites;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessageForUser([FromQuery] MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipiantUserName);
        Task<bool> SaveAllAsync();
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group>GetMessageGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);
    }
}
