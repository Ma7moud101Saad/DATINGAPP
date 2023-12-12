using API.DTOs;
using API.Entites;
using API.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface ILikeRepository
    {
        Task<UserLike> GetUserLike(int sourceId,int targetId); 
        Task<AppUser> GetUserWithLikes(int userId); 
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams); 
    }
}
