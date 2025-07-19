using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Service.Interface;
using Service.Service;
using System.Security.Claims;

namespace API_UsePrevention.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;

        public UsersController(IUserService userService, ICourseService courseService)
        {
            _userService = userService;
            _courseService = courseService;
        }

        [HttpGet("my-user")]
        [Authorize]
        public async Task<IActionResult> GetMyUserInfo()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            var dto = new MyUserDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Role = role
            };

            return Ok(dto);
        }

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Role name is required." });

            var role = await _userService.CreateRoleAsync(request);
            return Ok(new
            {
                message = "Role created successfully.",
                data = new
                {
                    role.Id,
                    role.Name
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.RegisterAsync(dto, isCustomer: true);
            return Ok(user);
        }


        [HttpPost("register-staff")]
        public async Task<IActionResult> RegisterStaff([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!dto.RoleId.HasValue)
                return BadRequest(new { message = "RoleId is required for staff registration." });

            var user = await _userService.RegisterAsync(dto, isCustomer: false);
            return Ok(user);
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Dto.Request.LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userService.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("role/{role}")]
        public async Task<IActionResult> GetByRole(int role)
        {
            var users = await _userService.GetByRoleAsync(role);
            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            if (id != user.Id) return BadRequest();
            await _userService.UpdateAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "Invalid token." });
            int userId = int.Parse(userIdClaim.Value);

            await _userService.ChangePasswordAsync(userId, request.NewPassword);
            return Ok(new { message = "Password changed successfully." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            try
            {
                await _userService.ForgotPasswordAsync(email);
                return Ok("Mật khẩu mới đã được gửi đến email của bạn.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("{userId}/registered-courses")]
        public async Task<IActionResult> GetRegisteredCourses(int userId)
        {
            var courses = await _courseService.GetCoursesByUserIdAsync(userId);
            return Ok(courses);
        }

        [HttpGet("consultant/{userId}")]
        public async Task<IActionResult> GetConsultantByUserId(int userId)
        {
            var consultant = await _userService.GetConsultantByUserIdAsync(userId);
            if (consultant == null)
                return NotFound(new { message = "Consultant not found for the specified UserId." });

            return Ok(new
            {
                consultant.Id,
                consultant.UserId,
                consultant.Degree,
                consultant.Specialty,
                consultant.WorkSchedule,
                User = new
                {
                    consultant.User.Id,
                    consultant.User.FullName,
                    consultant.User.Email
                }
            });
        }

    }
}
