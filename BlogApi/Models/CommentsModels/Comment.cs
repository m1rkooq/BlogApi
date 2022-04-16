using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Models.CommentsModels
{
    public class Comment
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public string CreateDate { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
    }
}
