using BlogApi.Models.BlogsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Services
{
    public interface IBlogServise
    {
        Task<List<Blog>> GetUserBlog(int UserId);
    }
}
