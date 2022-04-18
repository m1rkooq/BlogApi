using BlogApi.Models;
using BlogApi.Models.Admins;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Contorollers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAdminServise _service;
        public AdminController(IAdminServise service) =>
            _service = service;

        [HttpGet("GetDate")]
        public IActionResult GetPostPerDate([FromBody] AdminsRequestsDate model)
        {
            var query = _service.GetPostPerDate(model);
            if (query != null)
                return Ok(query);
            return BadRequest(new { message = "Not Found"});
        }
        [HttpGet("GetUser")]
        public IActionResult GetPostPerUserId([FromBody] AdminsRequestUser model)
        {
            var query = _service.GetPostPerUser(model.UserId);
            
            if(query != null)
                return Ok(query);
            return BadRequest(new { message = "Not Found" });

        }
        [HttpGet("GetTag")]
        public IActionResult GetPostPerTag([FromBody] AdminsRequestTags model)
        {
            var query = _service.GetPostPerTags(model);
            return Ok(query);
        }

        private UserResponce GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new UserResponce
                {
                    Id = Convert.ToInt32(userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value),
                    FirstName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,
                    Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;
        }
    }
}
