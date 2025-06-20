using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Service.Interface;
using Service.Service;
using System.Security.Claims;

namespace API_UsePrevention.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipationController : Controller
    {
        private readonly IParticipationService _service;

        public ParticipationController(IParticipationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] ParticipationCreateUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _service.CreateOrUpdateParticipationAsync(userId, dto);
            if (result == null)
                return BadRequest("Không thể tham gia chương trình ngoài thời gian hoạt động.");
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyParticipations()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _service.GetUserParticipationsAsync(userId);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> GetAll()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Không xác định được người dùng.");
            var result = await _service.GetAllParticipationsAsync();
            return Ok(result);
        }

    }
}
