
using BlogApi.Models.TagsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Services
{
    public interface ITagService
    {
        Task<int> CreateTags(Tags model);
        Task<bool> DeleteTags(Tags model);
        Task<List<Tags>> GetTagListByPostId(Tags model);
    }
}
