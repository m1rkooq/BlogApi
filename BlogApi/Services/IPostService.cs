using BlogApi.Models.PostsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Services
{
    public interface IPostService
    {
        IEnumerable<GetPostPesponse> GetPostByUserId(int UserId);
        IEnumerable<GetPostPesponse> GetPostByUserIdAndPostId(int UserId, int PostId);
        Task<List<PostResponse>> CreatePost(int UserId, PostCreate postCreate);       
        Task<List<PostResponse>> UpdatePost(int UserId, PostUpdate postUpdate);
        Task<int> DeletePost(int UserId, PostDelete postDelete);
        Task<bool> isTrue(int PostsId, int UserId);

    }
}
