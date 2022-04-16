using BlogApi.Helper;
using BlogApi.Models;
using BlogApi.Models.CommentsModels;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _service;

        public CommentController(ICommentService service,
            IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
        }

        [HttpGet("blogs/{UserId}/posts/{PostId}/Comments/{Id}")]
        public IActionResult GetCommentPyPostId([FromBody] Comment comment)
        {
            var query = _service.GetCommentsByPostId(comment);
            if(query != null)
            {
                return Ok(query);
            }
            return Ok($"No comments PostId: {comment.PostId}");
        }

            

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody]CommentCreate comment)
        {     
            var currentUser = GetCurrentUser();

            if(currentUser != null)
            {
                var command = await _service.CommentCreate(currentUser.Id, comment);
                if(command != null)
                {
                    return Ok(command);
                }
                return BadRequest(new { message = "Error!" });
            }
            return Ok("You Can't create comment!");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteComment([FromBody]CommentDelete comment)
        {
            var currentUser = GetCurrentUser();
            var check = await _service.isTrue(currentUser.Id, comment);

            if (currentUser != null && check != false)
            {
                var command = await _service.CommentDelete(currentUser.Id, comment);
                if(command != 0)
                    return Ok($"Delete success UserID: {currentUser.Id}, PostId: {comment.PostId}, CommentId: {comment.Id}");
            }
            return Ok("You Can't delete comment!");
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
