using Dto.Request;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Service.Interface;

namespace API_UsePrevention.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityProgramsController : Controller
    {

        private readonly ICommunityProgramService _service;

        public CommunityProgramsController(ICommunityProgramService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var programs = await _service.GetAllAsync();
            return Ok(programs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var program = await _service.GetByIdAsync(id);
            if (program == null) return NotFound();
            return Ok(program);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommunityProgramDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdProgram = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdProgram.Id }, createdProgram);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CommunityProgramDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, dto);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }
    }
    }
