using BlogApi.Models.LikesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Services
{
    public interface ILikeService
    {
        Task<bool> LikeCheck(int UserId, Likes model);
        Task UnLikePost(int UserId, Likes model);
        Task LikePost(int UserId, Likes model);
        Task<int> GetCountLikeByPostId(Likes model);
    }
}
