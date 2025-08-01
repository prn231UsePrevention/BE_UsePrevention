﻿using Dto.Request;
using Dto.Response;
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
        Task<User> RegisterAsync(RegisterUserDto dto, bool isCustomer = true);
        Task<User> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetByRoleAsync(int role);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<LoginResponseDto> LoginAsync(LoginRequest dto);
        Task ChangePasswordAsync(int userId, string newPassword);
        Task<Role> CreateRoleAsync(CreateRoleRequest role);
        Task ForgotPasswordAsync(string email);
        Task<Consultant> GetConsultantByUserIdAsync(int userId);
    }
}
