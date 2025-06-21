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
        private readonly IEmailService _emailService;
        public UserService(IUnitOfWork unitOfWork, IJwtService jwtService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        public async Task<User> RegisterAsync(RegisterUserDto dto, bool isCustomer = true)
        {
            int roleId = isCustomer ? 1 : dto.RoleId ?? throw new Exception("RoleId is required for non-customer registration.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                RoleId = roleId,
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

        public async Task<Role> CreateRoleAsync(CreateRoleRequest request)
        {
            var role = new Role
            {
                Name = request.Name
            };

            await _unitOfWork.Role.AddAsync(role);
            await _unitOfWork.CommitAsync();
            return role;
        }



        public async Task ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await _unitOfWork.User.GetByIdAsync(userId);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _unitOfWork.User.UpdateAsync(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = (await _unitOfWork.User.FindAsync(u => u.Email == email)).FirstOrDefault();
            if (user == null)
                throw new Exception("Email không tồn tại trong hệ thống.");

            var newPassword = GenerateRandomPassword();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _unitOfWork.User.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            var subject = "Khôi phục mật khẩu - Hệ thống phòng chống ma túy";
            var body = $"<p>Mật khẩu mới của bạn là: <strong>{newPassword}</strong></p>";

            await _emailService.SendEmailAsync(email, subject, body);
        }

        private string GenerateRandomPassword()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
