using SampleProj.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleProj.Services.Interfaces
{
    public interface IUserService : IBaseService<User>
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task<User> CreateAsync(User user, string password);
        Task<bool> ChangePassword(int userId, string newPassword);
        Task<bool> ChangePassword(int userId, string newPassword, string oldPassword);
    }
}
