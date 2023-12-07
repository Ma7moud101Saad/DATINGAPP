using API.Data;
using API.DTOs;
using API.Entites;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams) {
            var user = await _userRepository.GetUserByNameAsync(User.GetUserName());
            userParams.UserName = user.UserName;
            if (string.IsNullOrEmpty(userParams.Gender)) {
                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }
            var users=await _userRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.PageNumber, users.PageSize,users.TotalCount,users.TotalPages));
           return Ok(users);
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

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId) { 

           var user= await _userRepository.GetUserByNameAsync(User.GetUserName());
           if (user == null) return NotFound();

           var photo= user.Photos.FirstOrDefault(x => x.Id == photoId);
           if (photo == null) return NotFound();

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

           if(currentMain!= null) currentMain.IsMain = false;

            if (photo.IsMain) return BadRequest("this photo is already main");

            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest();
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId) {
            var user = await _userRepository.GetUserByNameAsync(User.GetUserName());

            var photo=user.Photos.FirstOrDefault(x=>x.Id== photoId);
            if (photo == null) return NotFound();

            if(photo.IsMain) return BadRequest("You can not delete the main photo");

            if (photo.PublicId != null) {
             var result= await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if(await _userRepository.SaveAllAsync())return NoContent();

            return BadRequest("problem delting a photo");
        }
    }
}
