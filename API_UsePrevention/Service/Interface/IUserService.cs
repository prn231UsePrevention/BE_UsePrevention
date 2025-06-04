using Dto.Request;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterUserDto user);
        Task<User> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetByRoleAsync(string role);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<User> LoginAsync(string email, string password);
        Task ChangePasswordAsync(int userId, string newPassword);
    }
}
