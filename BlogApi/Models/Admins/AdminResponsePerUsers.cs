using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Models.Admins
{
    public class AdminResponsePerUsers
    {
        public List<UserResponce> Users { get; set; }
        public int CountPostPerUser { get; set; }

    }
}
