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
            var normalizedEmail = dto.Email.Trim().ToLower();

            // Check if email already exists
            var existingUser = await _unitOfWork.User.Query()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
            if (existingUser != null)
                throw new Exception("Email is already registered.");

            

            int roleId;

            if (isCustomer)
            {
                // Get role with Name = "Customer"
                var customerRole = await _unitOfWork.Role.Query()
                    .FirstOrDefaultAsync(r => r.Name.ToLower() == "customer");

                if (customerRole == null)
                    throw new Exception("Role 'Customer' does not exist.");

                roleId = customerRole.Id;
            }
            else
            {
                if (!dto.RoleId.HasValue)
                    throw new ArgumentException("RoleId is required for staff registration.");

                // Optional: verify the role exists in DB
                var roleExists = await _unitOfWork.Role.Query()
                    .AnyAsync(r => r.Id == dto.RoleId.Value);

                if (!roleExists)
                    throw new Exception($"Role with ID {dto.RoleId.Value} does not exist.");

                roleId = dto.RoleId.Value;
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                RoleId = roleId
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

        public async Task<Consultant> GetConsultantByUserIdAsync(int userId)
        {
            return await _unitOfWork.Consultant
                .Query()
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        } 
    }
}
