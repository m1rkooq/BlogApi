using BlogApi.Models;
using BlogApi.Models.PostsModels;

using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Contorollers
{
    //[Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService _service;

        public PostController(IPostService service)
        {
            _service = service;

        }

        [AllowAnonymous]
        [HttpGet("blogs/{UserId}/posts/")]
        public IActionResult GetPostsByUserId(int UserId)
        {
            var query = _service.GetPostByUserId(UserId);
            if(query != null)
            {                
                return Ok(query);
            }
            return BadRequest(new { message = "Not found!"});
        }

        [AllowAnonymous]
        [HttpGet("blogs/{UserId}/posts/{PostId}")]
        public IActionResult GetPistsByUserIdAndPostId(int UserId, int PostId)
        {
            var query = _service.GetPostByUserIdAndPostId(UserId, PostId);
            if (query != null)
            {
                return Ok(query);
            }
            return BadRequest(new { message = "Not found!" });
        }


        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostCreate post)
        {
            var currentUser = GetCurrentUser();
            if (currentUser != null)
            {
                var command = await _service.CreatePost(currentUser.Id, post);
                if(command != null)
                {
                    return Ok(command);
                }
                return BadRequest(new { message = "Post don't create!" });
            }
            return BadRequest(new { message = "Not found!" });
        }
        [HttpPut]
        public async Task<IActionResult> UpdatePost([FromBody] PostUpdate post)
        {
            var currentUser = GetCurrentUser();
            var check = await _service.isTrue(post.Id, currentUser.Id);
            if(check != false)
            {
                var command = await _service.UpdatePost(currentUser.Id, post);

                if (command != 0)
                {
                    return Ok($"Post was edit PostId: {post.Id}. UserId: {currentUser.Id}");
                }
                return BadRequest(new { message = "Post don't edite!" });
            }

            return BadRequest(new { message = "U can't edit that post!" });
        }

        
        [HttpDelete]
        public async Task<IActionResult> DeletePost([FromBody] PostDelete post)
        {
            var currentUser = GetCurrentUser();
            var check = await _service.isTrue(post.Id, currentUser.Id);

            if (check != false)
            {
                var command = await _service.DeletePost(currentUser.Id, post);

                if(command != 0)
                {
                    return Ok($"Post was delete PostId: {post.Id}. UserId: {currentUser.Id}");
                }
                return NoContent();
            }

            return BadRequest(new { message = "U can't delete that post!" });
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
