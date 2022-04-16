using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlogApi.Models.LikesModels
{
    public class Likes
    {
        public int LikeId { get; set; }
        public int PostId { get; set; }      
        //public int UserId { get; set; }
    }
}
