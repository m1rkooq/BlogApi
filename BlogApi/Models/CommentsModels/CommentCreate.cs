using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Models.CommentsModels
{
    public class CommentCreate
    {      
        public string CommentText { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public int PostId { get; set; }
    }
}
