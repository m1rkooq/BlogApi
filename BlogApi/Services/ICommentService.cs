using BlogApi.Models.CommentsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Services
{
    public interface ICommentService
    {
        Task<List<CommentResponse>> CommentCreate(int UserId, CommentCreate commentCreate);
        Task<int> CommentDelete(int UserId, CommentDelete commentDelete);
        Task<bool> isTrue(int UserId, CommentDelete commentDelete);
        IEnumerable<CommentResponse> GetCommentsByPostId(Comment comment);

    }
}
