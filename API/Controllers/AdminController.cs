using API.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [Authorize(policy: "RequiredAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles() {
           var users=await _userManager.Users.
                OrderBy(x=>x.Created)
                .Select(x=>new {
                        x.Id,
                      UserName = x.UserName,
                        roles=x.UserRoles.Select(x=>x.AppRole.Name).ToList()
                }).ToListAsync();
            return Ok(users);
        }

        [Authorize(policy: "RequiredAdminRole")]
        [HttpPost("edit-roles/{userName}")]
        public async Task<ActionResult> editRoles(string userName, [FromQuery] string roles) {

            if (string.IsNullOrEmpty(roles)) return BadRequest("you must select at least one role");
            var user=await _userManager.FindByNameAsync(userName);

            if(user == null)return NotFound("user not found");

            var slectedRoles =roles.Split(",");

            var userRoles=await _userManager.GetRolesAsync(user);

            var result=await _userManager.AddToRolesAsync(user, slectedRoles.Except(userRoles));
            if (!result.Succeeded) return BadRequest("Faild to Add to Roles");

            result =await _userManager.RemoveFromRolesAsync(user,userRoles.Except(slectedRoles));
            if (!result.Succeeded) return BadRequest("Faild to romve from Roles");

            return Ok(await _userManager.GetRolesAsync(user));
            
        }

        [Authorize(policy:"ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotoToModeration() {
            return Ok("Admins or moderators can see this");
        }
    }
}
