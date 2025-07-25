using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Service.Interface;
using System.Security.Claims;

namespace API_UsePrevention.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var feedbacks = await _feedbackService.GetAllAsync();
            return Ok(feedbacks);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var feedback = await _feedbackService.GetByIdAsync(id);
            if (feedback == null) return NotFound();
            return Ok(feedback);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] CreateFeedbackDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var created = await _feedbackService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer")]

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _feedbackService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Customer")]

        public async Task<IActionResult> Update(int id, [FromBody] UpdateFeedbackDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var updated = await _feedbackService.UpdateAsync(id, dto, userId);
            if (updated == null)
                return NotFound(new { message = "Feedback not found" });

            return Ok(updated);
        }


    }
}
