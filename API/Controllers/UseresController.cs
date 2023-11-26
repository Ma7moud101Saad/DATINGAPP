using API.Data;
using API.DTOs;
using API.Entites;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IphotoService _photoService;

        public UsersController(IUserRepository userRepository,
            IMapper mapper,
            IphotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers() { 
        
            return Ok(await _userRepository.GetMembersAsync());

        }
        [HttpGet("{userName}")]
        public async Task<ActionResult<MemberDto>> GetUser(string userName)
        { return Ok(await _userRepository.GetMemberByNameAsync(userName));
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) {
            var user =await _userRepository.GetUserByNameAsync(User.GetUserName());
            if (user == null) return NotFound();
            _mapper.Map(memberUpdateDto, user);
           if(await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest();
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult> AddPhoto(IFormFile file) {
          var user = await _userRepository.GetUserByNameAsync(User.GetUserName());
            if (user == null) return NotFound();
            var uploadResult= await _photoService.UploadPhotoAsync(file);
            Photo photo = new Photo()
            {
                Url = uploadResult.SecureUrl.AbsoluteUri,
                PublicId = uploadResult.PublicId
            };
            if(user.Photos.Count ==0)photo.IsMain = true;
            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync()) {
                return CreatedAtAction(nameof(GetUser),new { userName = user.UserName }, _mapper.Map<PhotoDto>( photo));
            }

            return BadRequest();
        }
    }
}
