using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using Dto.Request;
using Service.Interface.Service.Interface;

namespace API_UsePrevention.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class AssessmentAnswerController : ControllerBase
    {
        private readonly IAssessmentAnswerService _service;

        public AssessmentAnswerController(IAssessmentAnswerService service)
        {
            _service = service;
        }

       
        [HttpGet("result/{resultId}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetAllByResultId(int resultId)
        {
            var answers = await _service.GetAllByResultIdAsync(resultId);
            return Ok(answers);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var answer = await _service.GetByIdAsync(id);
            if (answer == null)
                return NotFound();
            return Ok(answer);
        }

        [HttpPost]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Create([FromBody] AssessmentAnswerCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.AddAsync(dto);
            return Ok(new { message = "Created successfully" });
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Update(int id, [FromBody] AssessmentAnswerUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(id, dto);
            return Ok(new { message = "Updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Deleted successfully" });
        }
    }
}
