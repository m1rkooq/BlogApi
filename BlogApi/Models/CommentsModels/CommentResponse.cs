using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Models.CommentsModels
{
    public class CommentResponse
    {
        public List<UserResponce> Users { get; set; }
        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public string CreateTime { get; set; } //= DateTime.Now;
        public int PostId { get; set; }


    }
}
