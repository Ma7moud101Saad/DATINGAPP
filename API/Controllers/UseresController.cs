﻿using API.Data;
using API.DTOs;
using API.Entites;
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

        public UsersController(IUserRepository userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
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
           var userName= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user =await _userRepository.GetUserByNameAsync(userName);
            if (user == null) return NotFound();
            _mapper.Map(memberUpdateDto, user);
           if(await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest();
        }
        
    }
}
