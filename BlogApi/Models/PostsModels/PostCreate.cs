using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Models.PostsModels
{
    public class PostCreate
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;     
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public string[] tags { get; set; }
    }
}
