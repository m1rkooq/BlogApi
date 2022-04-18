using BlogApi.Models.Admins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Services
{
    public interface IAdminServise
    {
        IEnumerable<AdminResponsePerDate> GetPostPerDate(AdminsRequestsDate model);
        IEnumerable<AdminResponsePerUsers> GetPostPerUser(int UserId);
        List<AdminsResponsePerTags> GetPostPerTags(AdminsRequestTags model);
    }
}
