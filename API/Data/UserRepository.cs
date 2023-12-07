using API.DTOs;
using API.Entites;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public UserRepository(DataContext dataContext,IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetMemberByNameAsync(string userName)
        {
            return await _dataContext.Users
                 .Where(x => x.UserName.ToLower() == userName.ToLower())
                 .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                 .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            var query=  _dataContext.Users.AsQueryable();
            query = query.Where(x => x.UserName != userParams.UserName);
            query = query.Where(x => x.Gender == userParams.Gender);
            query = query.Where(x => x.DateOfBirth <= maxDob && x.DateOfBirth>=minDob);
            query = userParams.OrderBy switch { 
            "created"=>query.OrderByDescending(u=>u.Created),
            _=>query.OrderByDescending(u=>u.LastActive)};

            //query = userParams.OrderBy == "created" ? query.OrderByDescending(u => u.Created)
            //    : query.OrderByDescending(u => u.LastActive);
            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).
                AsNoTracking(), userParams.PageNumber, userParams.PageSize);
        }

        public async Task<AppUser> GetUserById(int id)
        {
            return await _dataContext.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByNameAsync(string userName)
        {
            return await _dataContext.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _dataContext.Users
                .Include(x=>x.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0 ;
        }

        public void Update(AppUser appUser)
        {
            _dataContext.Entry(appUser).State = EntityState.Modified;
        }
    }
}
