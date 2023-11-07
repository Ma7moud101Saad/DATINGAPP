using API.Data;
using API.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _dataContext;

        public BuggyController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Authorize]
        [HttpGet("Auth")]
        public ActionResult<string> GetSecret() {
            return "Secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound(string secret)
        {
          var thing= _dataContext.Users.Find(-1);
          if (thing == null) return NotFound();
          return thing;
        }

        [HttpGet("server-error")]
        public ActionResult<AppUser> GetServerError() {
         
                var thing = _dataContext.Users.Find(-1);
                thing.ToString();
                return thing;
            
        }

        [HttpGet("bad-request")]
        public ActionResult<string> BadRequest() {
            return BadRequest("this is not a good request");
        }


    }
}
