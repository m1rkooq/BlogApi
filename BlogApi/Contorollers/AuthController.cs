using BlogApi.Helper;
using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Contorollers
{
    
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;            
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserRequest userRequest)
        {
            var user = _userService.Autheticate(userRequest);
            
            if (user != null)
            {
                var token = _configuration.GenerateToken(user);
                return Ok(token);
            }
            return NotFound("User not found!");

        }


        [AllowAnonymous]
        [HttpPost("register")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> register([FromBody] UserRegister userRegister)
        {
            var users = await _userService.Register(userRegister);

            if (users != 0)
            {
                return Ok($"User was registration. USER ID: {users}");
            }

            return BadRequest(new { mewssage = "Didn't register!" });
        }


    }
}
