using API.DTOs;
using API.Entites;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser appUser);
        Task<bool>SaveAllAsync();
        Task<AppUser> GetUserById(int id);
        Task<AppUser> GetUserByNameAsync(string userName);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<MemberDto> GetMemberByNameAsync(string userName);
        Task<IEnumerable<MemberDto>> GetMembersAsync();


    }
}
