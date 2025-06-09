using Dto.Request;
using Dto.Response;
using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Repository.Repositories;
using Repository.UWO;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;

        public UserService(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<User> RegisterAsync(RegisterUserDto dto)
        {
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                RoleId = 2,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _unitOfWork.User.AddAsync(user);
            await _unitOfWork.CommitAsync();

            return user;
        }


        public async Task<User> GetByIdAsync(int id)
        {
            return await _unitOfWork.User.GetByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _unitOfWork.User.GetAll();
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(int role)
        {
            return await _unitOfWork.User.FindAsync(u => u.RoleId == role);
        }

        public async Task UpdateAsync(User user)
        {
            await _unitOfWork.User.UpdateAsync(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _unitOfWork.User.GetByIdAsync(id);
            if (user != null)
            {
                await _unitOfWork.User.DeleteAsync(user);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequest dto)
        {
            var user = await _unitOfWork.User.Query()
                .Include(u => u.Role)  
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

            var token = _jwtService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
            };
        }


        public async Task ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await _unitOfWork.User.GetByIdAsync(userId);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _unitOfWork.User.UpdateAsync(user);
            await _unitOfWork.CommitAsync();
        }
    }
}
