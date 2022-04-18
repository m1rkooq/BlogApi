using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Models.Admins
{
    public class AdminResponsePerDate
    {       
        public string Date { get; set; }
        public int CountPostPerday { get; set; }
    }
}
