using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Models.Admins
{
    public class AdminsResponsePerTags
    {
        public string Tags { get; set; }
        public int CountPosts { get; set; }
    }
}
