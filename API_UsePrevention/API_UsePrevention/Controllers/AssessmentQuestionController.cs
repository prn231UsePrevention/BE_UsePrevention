﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using Dto.Request;

namespace API_UsePrevention.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class AssessmentQuestionController : ControllerBase
    {
        private readonly IAssessmentQuestionService _service;

        public AssessmentQuestionController(IAssessmentQuestionService service)
        {
            _service = service;
        }


        [HttpGet]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetAll()
        {
            var questions = await _service.GetAllAsync();
            return Ok(questions);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _service.GetByIdAsync(id);
            if (question == null)
                return NotFound();
            return Ok(question);
        }

        [HttpGet("assessment/{assessmentId}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetByAssessmentId(int assessmentId)
        {
            var questions = await _service.GetByAssessmentIdAsync(assessmentId);
            return Ok(questions);
        }

   
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] AssessmentQuestionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.AddAsync(dto);
            return Ok(new { message = "Created successfully" });
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] AssessmentQuestionUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(id, dto);
            return Ok(new { message = "Updated successfully" });
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Deleted successfully" });
        }
    }
}
