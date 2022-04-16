using BlogApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.Services
{
    public interface IUserService
    {
        User Autheticate(UserRequest userRequest);
        Task<int> Register(UserRegister userRegister);
    }
}
