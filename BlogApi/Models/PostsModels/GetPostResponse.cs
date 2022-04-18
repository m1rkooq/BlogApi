using BlogApi.Models.TagsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Models.PostsModels
{
    public class GetPostResponse
    {
        public List<UserResponce> user { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreatedDate { get; set; }
        public string UpdateDate { get; set; }
        public int CountComments { get; set; }
        public int CountLike { get; set; }
        public List<Tags> TagList { get; set; }        
    }
}
