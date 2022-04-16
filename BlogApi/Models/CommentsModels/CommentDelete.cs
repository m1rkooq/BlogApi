using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Models.CommentsModels
{
    public class CommentDelete
    {
        public int Id { get; set; }
        public int PostId { get; set; }
    }
}
