using BlogApi.Models;
using BlogApi.Models.LikesModels;
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
    public class LikeController: ControllerBase
    {
        private readonly ILikeService _service;
        public LikeController(ILikeService service) =>
            _service = service;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCountLikesByPostId([FromBody] Likes model)
        {
            var query = await _service.GetCountLikeByPostId(model);
            return Ok($"Count like on postId: {model.PostId} - {query}");
            
        }

        [HttpPost]
        public async Task<IActionResult> LikePost([FromBody] Likes model)
        {
            var currentUser = GetCurrentUser();
            var like = await _service.LikeCheck(currentUser.Id, model);

            if (like == true)
                return Ok($"UnLike PostId: {model.PostId}, UserId: {currentUser.Id}, LikeId: {model.LikeId}");
            else
                return Ok($"Like PostId: {model.PostId}, UserId: {currentUser.Id}, LikeId: {model.LikeId}");
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
