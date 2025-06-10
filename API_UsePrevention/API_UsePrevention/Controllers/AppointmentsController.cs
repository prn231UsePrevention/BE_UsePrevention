using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace API_UsePrevention.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("consultants")]
        [Authorize(Roles = "Member,Consultant,Manager,Admin")]
        public async Task<ActionResult> GetConsultants()
        {
            try
            {
                var consultants = await _appointmentService.GetConsultantsAsync();
                return Ok(consultants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving consultants: {ex.Message}");
            }
        }

        [HttpGet("available")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult> GetAvailableSlots([FromQuery] GetAvailableSlotsRequestDto request)
        {
            try
            {
                var slots = await _appointmentService.GetAvailableSlotsAsync(request);
                return Ok(slots);
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
                return StatusCode(500, $"An error occurred while retrieving available slots: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult> CreateAppointment([FromBody] CreateAppointmentRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var appointment = await _appointmentService.CreateAppointmentAsync(userId, request);
                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
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
                return StatusCode(500, $"An error occurred while creating the appointment: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Member,Manager,Admin")]
        public async Task<ActionResult> GetUserAppointments(int userId)
        {
            try
            {
                var appointments = await _appointmentService.GetUserAppointmentsAsync(userId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving user appointments: {ex.Message}");
            }
        }

        [HttpGet("consultant/{consultantId}")]
        [Authorize(Roles = "Consultant,Manager,Admin")]
        public async Task<ActionResult> GetConsultantAppointments(int consultantId)
        {
            try
            {
                var appointments = await _appointmentService.GetConsultantAppointmentsAsync(consultantId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving consultant appointments: {ex.Message}");
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateAppointmentStatusRequestDto request)
        {
            try
            {
                await _appointmentService.UpdateAppointmentStatusAsync(id, request);
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
                return StatusCode(500, $"An error occurred while updating the appointment status: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Member,Manager,Admin")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userRole = User.FindFirst("role")?.Value;
                await _appointmentService.CancelAppointmentAsync(id, userId, userRole);
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
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while cancelling the appointment: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Member,Consultant,Manager,Admin")]
        public async Task<ActionResult> GetAppointment(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                return Ok(appointment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the appointment: {ex.Message}");
            }
        }
    }
}