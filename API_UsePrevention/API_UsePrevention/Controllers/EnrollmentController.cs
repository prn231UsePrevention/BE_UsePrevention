using Dto.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Service.Interface;

namespace API_UsePrevention.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // GET: api/enrollment
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _enrollmentService.GetAllAsync();
            return Ok(result);
        }

        // GET: api/enrollment/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _enrollmentService.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST: api/enrollment
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EnrollmentRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _enrollmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/enrollment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EnrollmentRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _enrollmentService.UpdateAsync(id, dto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/enrollment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _enrollmentService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
