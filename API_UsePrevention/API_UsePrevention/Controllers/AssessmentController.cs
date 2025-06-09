using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using Dto.Request;

namespace API_UsePrevention.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class AssessmentController : ControllerBase
    {
        private readonly IAssessmentService _assessmentService;

        public AssessmentController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }


        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _assessmentService.GetAllAssessmentsAsync();
            return Ok(list);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var assessment = await _assessmentService.GetAssessmentByIdAsync(id);
            if (assessment == null)
                return NotFound();
            return Ok(assessment);
        }


        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create([FromBody] AssessmentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _assessmentService.CreateAssessmentAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(int id, [FromBody] AssessmentUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _assessmentService.UpdateAssessmentAsync(id, dto);
            if (!success)
                return NotFound();
            return Ok(new { message = "Updated successfully" });
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _assessmentService.DeleteAssessmentAsync(id);
            if (!success)
                return NotFound();
            return Ok(new { message = "Deleted successfully" });
        }


        [HttpPost("submit")]
        [Authorize(Roles = "USER,ADMIN")]
        public async Task<IActionResult> Submit([FromBody] AssessmentSubmitDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
            var result = await _assessmentService.SubmitAssessmentAsync(dto.UserId, dto.AssessmentId, dto.Answers);
            return Ok(result);
        }

        [HttpGet("results/user/{userId}")]
        [Authorize(Roles = "USER,ADMIN")]
        public async Task<IActionResult> GetResultsByUser(int userId)
        {
            var results = await _assessmentService.GetUserAssessmentResultsAsync(userId);
            return Ok(results);
        }


        [HttpGet("result/{resultId}")]
        [Authorize(Roles = "USER,ADMIN")]
        public async Task<IActionResult> GetResultById(int resultId)
        {
            var result = await _assessmentService.GetAssessmentResultByIdAsync(resultId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
