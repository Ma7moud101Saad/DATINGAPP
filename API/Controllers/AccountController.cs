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
        private readonly DataContext _dbContext;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(
            DataContext dbContext,
            ITokenService tokenService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName)) return BadRequest("User name Already Exist");
            using var hmac = new HMACSHA512();
            AppUser user = new AppUser();

            _mapper.Map(registerDto, user);

            user.UserName = registerDto.UserName;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;
            
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
          
            return new UserDto {
                UserName=user.UserName,
                Token= _tokenService.CreateToken(user),
                KnownAs=user.KnownAs
            };

        }

        private async Task<bool> UserExists(string UserName) { 
            return await _dbContext.Users.AnyAsync(u => u.UserName == UserName);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) {

           var user= await _dbContext.Users
                .Include(x=>x.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if (user == null) return Unauthorized("User Does not exist");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++) {
                if (user.PasswordHash[i] != computedHash[i]) return Unauthorized("Invalid password");
            }
            return new UserDto {
                UserName=user.UserName,
                Token= _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
        }
    }
}
