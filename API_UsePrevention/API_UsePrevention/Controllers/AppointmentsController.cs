using Dto.Request;
using Dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.UWO;
using Service.Interface;

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

        [HttpGet("available-slots")]
        public async Task<ActionResult<IEnumerable<AvailableSlotResponseDto>>> GetAvailableSlots([FromQuery] GetAvailableSlotsRequestDto request)
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
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var isConsultant = (await _unitOfWork.Consultant.GetByIdAsync(userId)) != null;
                if (isConsultant)
                    return Unauthorized("Consultants cannot book slots");

                var appointment = await _appointmentService.BookSlotAsync(userId, request);
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
                return StatusCode(500, $"An error occurred while booking the slot: {ex.Message}");
            }
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetUserAppointments()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value);
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
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var isConsultant = (await _unitOfWork.Consultant.GetByIdAsync(userId)) != null;
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
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var isConsultant = (await _unitOfWork.Consultant.GetByIdAsync(userId)) != null;
                if (!isConsultant)
                    return Unauthorized("Only consultants can update appointment status");

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
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var userRole = (await _unitOfWork.Consultant.GetByIdAsync(userId)) != null ? "Consultant" : "Member";
                await _appointmentService.CancelAppointmentAsync(appointmentId, userId, userRole);
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
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var consultant = await _unitOfWork.Consultant.FindAsync(c => c.UserId == userId);
                if (!consultant.Any())
                    return Unauthorized("Only consultants can create slots");

                await _appointmentService.CreateSlotAsync(userId, request);
                return CreatedAtAction(nameof(GetAvailableSlots), new { consultantId = consultant.First().Id, date = request.StartTime.Date }, null);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
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
                return StatusCode(500, $"An error occurred while creating the slot: {ex.Message}");
            }
        }
    }
}