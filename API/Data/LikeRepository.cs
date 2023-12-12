using API.DTOs;
using API.Entites;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class LikeRepository : ILikeRepository
    {
        private readonly DataContext _context;

        public LikeRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int sourceId, int targetId)
        {
            return await _context.UserLike.FindAsync(sourceId, targetId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users =  _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var query =  _context.UserLike.AsQueryable();
            if (likesParams.Predicate == "liked")
            {
              var userLikes=  query.Where(x => x.SourceUserId == likesParams.UserId);
            users = userLikes.Select(ul => ul.TargetUser);
            }
            else if (likesParams.Predicate == "likedBy")
            {
                var userLikes = query.Where(x => x.TargetUserId == likesParams.UserId);
                users = userLikes.Select(ul => ul.SourceUser);
            }
            var userLikesQuery=  users.Select(user=>new LikeDto
            {
                Id = user.Id,
                UserName=user.UserName,
                Age=user.DateOfBirth.CalculateAge(),
                KnownAs = user.KnownAs,
                PhotoUrl=user.Photos.FirstOrDefault(x=>x.IsMain).Url,
                City=user.City
            });

            return await PagedList<LikeDto>.CreateAsync(userLikesQuery, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(u=>u.LikedUsers)
                .FirstOrDefaultAsync(u=>u.Id==userId);
        }
    }
}
