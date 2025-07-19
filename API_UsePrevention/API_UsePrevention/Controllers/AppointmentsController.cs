using Dto.Request;
using Dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.UWO;
using Service.Interface;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_UsePrevention.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentsController(IAppointmentService appointmentService, IUnitOfWork unitOfWork)
        {
            _appointmentService = appointmentService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("consultants")]
        public async Task<ActionResult<IEnumerable<ConsultantResponseDto>>> GetConsultants()
        {
            try
            {
                var consultants = await _appointmentService.GetConsultantsAsync();
                return Ok(consultants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // SỬA LỖI 1: Thay đổi chữ ký của Action này
        // Thay vì nhận một đối tượng phức tạp [FromQuery], chúng ta nhận các tham số đơn giản.
        // Điều này cho phép `CreatedAtAction` trong phương thức `CreateSlot` có thể tìm thấy route này một cách chính xác.
        [HttpGet("available-slots")]
        public async Task<ActionResult<IEnumerable<AvailableSlotResponseDto>>> GetAvailableSlots([FromQuery] int consultantId, [FromQuery] DateTime date)
        {
            try
            {
                // Tạo lại đối tượng DTO để truyền vào service
                var requestDto = new GetAvailableSlotsRequestDto
                {
                    ConsultantId = consultantId,
                    Date = date
                };
                var slots = await _appointmentService.GetAvailableSlotsAsync(requestDto);
                return Ok(slots);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("all-available-slots")]
        public async Task<ActionResult<IEnumerable<AvailableSlotResponseDto>>> GetAllAvailableSlots()
        {
            try
            {
                var slots = await _appointmentService.GetAllAvailableSlotsAsync();
                return Ok(slots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("book")]
        [Authorize]
        public async Task<ActionResult> BookSlot([FromBody] BookSlotRequestDto request)
        {
            try
            {
                var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(emailClaim))
                    return Unauthorized("Email claim is missing in the JWT token");

                var users = _unitOfWork.User.Find(u => u.Email == emailClaim);
                var user = users.FirstOrDefault();
                if (user == null)
                    return Unauthorized("User not found");

                var userId = user.Id;
                if (user.RoleId == 5) // Consultant không được đặt lịch
                    return Unauthorized("Consultants (RoleId = 5) cannot book slots");

                var appointment = await _appointmentService.BookSlotAsync(userId, request);

                // SỬA LỖI 2: Sửa tên tham số route
                // Tên tham số trong route values (`appointmentId`) phải khớp chính xác với tên tham số trong phương thức `GetAppointment` (`int appointmentId`).
                // Trước đó là `id`, giờ đã sửa thành `appointmentId`.
                return CreatedAtAction(nameof(GetAppointment), new { appointmentId = appointment.Id }, appointment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while booking the slot: {ex.Message}");
            }
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetUserAppointments()
        {
            try
            {
                var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(emailClaim))
                    return Unauthorized("Email claim is missing in the JWT token");

                var users = _unitOfWork.User.Find(u => u.Email == emailClaim);
                var user = users.FirstOrDefault();
                if (user == null)
                    return Unauthorized("User not found");

                var userId = user.Id;
                var appointments = await _appointmentService.GetUserAppointmentsAsync(userId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("consultant/{consultantId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetConsultantAppointments(int consultantId)
        {
            try
            {
                var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(emailClaim))
                    return Unauthorized("Email claim is missing in the JWT token");

                var users = _unitOfWork.User.Find(u => u.Email == emailClaim);
                var user = users.FirstOrDefault();
                if (user == null)
                    return Unauthorized("User not found");

                var userId = user.Id;
                var isConsultant = (await _unitOfWork.Consultant.FindAsync(c => c.UserId == userId)).Any();
                if (!isConsultant && userId != consultantId)
                    return Unauthorized("Not authorized to view consultant appointments");

                var appointments = await _appointmentService.GetConsultantAppointmentsAsync(consultantId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{appointmentId}/status")]
        [Authorize]
        public async Task<ActionResult> UpdateAppointmentStatus(int appointmentId, [FromBody] UpdateAppointmentStatusRequestDto request)
        {
            try
            {
                var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(emailClaim))
                    return Unauthorized("Email claim is missing in the JWT token");

                var users = _unitOfWork.User.Find(u => u.Email == emailClaim);
                var user = users.FirstOrDefault();
                if (user == null || user.RoleId != 5) // Chỉ Consultant (RoleId = 5) được cập nhật trạng thái
                    return Unauthorized("Only consultants (RoleId = 5) can update appointment status");

                await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, request);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{appointmentId}/cancel")]
        [Authorize]
        public async Task<ActionResult> CancelAppointment(int appointmentId)
        {
            try
            {
                var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(emailClaim))
                    return Unauthorized("Email claim is missing in the JWT token");

                var users = _unitOfWork.User.Find(u => u.Email == emailClaim);
                var user = users.FirstOrDefault();
                if (user == null)
                    return Unauthorized("User not found");

                var userRole = user.RoleId == 5 ? "Consultant" : "Member";
                await _appointmentService.CancelAppointmentAsync(appointmentId, user.Id, userRole);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{appointmentId}")]
        [Authorize]
        public async Task<ActionResult<AppointmentResponseDto>> GetAppointment(int appointmentId)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
                // Bạn nên kiểm tra appointment có null không trước khi trả về
                if (appointment == null)
                {
                    return NotFound($"Appointment with ID {appointmentId} not found.");
                }
                return Ok(appointment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("slots")]
        [Authorize]
        public async Task<ActionResult> CreateSlot([FromBody] CreateSlotRequestDto request)
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(emailClaim))
                return Unauthorized("Email claim is missing in the JWT token");

            var users = _unitOfWork.User.Find(u => u.Email == emailClaim);
            var user = users.FirstOrDefault();
            if (user == null)
                return Unauthorized("User not found");

            var userId = user.Id;
            if (user.RoleId != 5) // Kiểm tra role Consultant
                return Unauthorized("Only consultants (RoleId = 5) can create slots");

            var consultant = await _unitOfWork.Consultant.FindAsync(c => c.UserId == userId);
            var firstConsultant = consultant.FirstOrDefault();
            if (firstConsultant == null)
                return Unauthorized("User is not a registered consultant");

            await _appointmentService.CreateSlotAsync(userId, request);

            // Lời gọi này giờ sẽ hoạt động vì chữ ký của `GetAvailableSlots` đã được thay đổi để khớp.
            return CreatedAtAction(nameof(GetAvailableSlots), new { consultantId = firstConsultant.Id, date = request.StartTime.Date }, null);
        }
    }
}
