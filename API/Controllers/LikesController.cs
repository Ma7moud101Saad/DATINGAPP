using API.Data;
using API.Entites;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : BaseApiController
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IUserRepository _userRepository;

        public LikesController(ILikeRepository likeRepository,
            IUserRepository userRepository)
        {
            _likeRepository = likeRepository;
            _userRepository = userRepository;
        }
        [HttpPost("{userName}")]
        public async Task<IActionResult> AddLike(string userName) { 
            int sourceUserId = User.GetUserId();
            var sourceUser = await _likeRepository.GetUserWithLikes(sourceUserId);

            var likedUser = await _userRepository.GetUserByNameAsync(userName);
            if (sourceUser == null) return NotFound();

            if (sourceUser.UserName == likedUser.UserName) return BadRequest("You can`t like yourself");

           var userLike= await _likeRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("you already liked this user");

            var like = new UserLike() {
                SourceUserId = sourceUserId,
                TargetUserId   = likedUser.Id
            };
            sourceUser.LikedUsers.Add(like);
            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Faild to like User");
        }
        [HttpGet]
        public async Task<IActionResult> GetUserLike([FromQuery]LikesParams likesParams) {
            likesParams.UserId= User.GetUserId();
            var users= await _likeRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.PageNumber, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }
    }
}
