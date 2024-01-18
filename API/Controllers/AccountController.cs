using API.Data;
using API.DTOs;
using API.Entites;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(
            ITokenService tokenService,
            IMapper mapper,
            UserManager<AppUser>userManager)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager = userManager;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName)) return BadRequest("User name Already Exist");
        
            AppUser user = new AppUser();

            _mapper.Map(registerDto, user);

            user.UserName = registerDto.UserName;

            await _userManager.CreateAsync(user,registerDto.Password);
            await _userManager.AddToRoleAsync(user, "Member");
            return new UserDto {
                UserName=user.UserName,
                Token=await _tokenService.CreateToken(user),
                KnownAs=user.KnownAs,
                Gender=user.Gender
            };
        }

        private async Task<bool> UserExists(string UserName) {
            return await _userManager.Users.AllAsync(u => u.UserName == UserName);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) {

           var user= await _userManager.Users
                .Include(x=>x.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if (user == null) return Unauthorized("Invalid User Name");

           var result= await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result) return Unauthorized("invalid password");

            return new UserDto {
                UserName=user.UserName,
                Token=await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                Gender = user.Gender
            };
        }
    }
}
