using AutoMapper;
using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Service.Interface;

namespace API_UsePrevention.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultantController : ControllerBase
    {
        private readonly IConsultantService _consultantService;
        private readonly IMapper _mapper;

        public ConsultantController(IConsultantService consultantService, IMapper mapper)
        {
            _consultantService = consultantService;
            _mapper = mapper;
        }

        [HttpGet]
        
        public async Task<IActionResult> GetAll()
        {
            var consultants = await _consultantService.GetAllAsync();
            return Ok(consultants);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var consultant = await _consultantService.GetByIdAsync(id);
            if (consultant == null) return NotFound();
            return Ok(consultant);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create([FromBody] ConsultantCreateDto dto)
        {
            var consultant = _mapper.Map<Consultant>(dto);
            await _consultantService.AddAsync(consultant);
            return Ok(consultant);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Update(int id, [FromBody] ConsultantUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            var consultant = _mapper.Map<Consultant>(dto);
            await _consultantService.UpdateAsync(consultant);
            return Ok(consultant);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            await _consultantService.DeleteAsync(id);
            return NoContent();
        }
    }
}
