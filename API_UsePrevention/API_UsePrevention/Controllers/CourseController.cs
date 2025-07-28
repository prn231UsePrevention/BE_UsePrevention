using Azure.Core;
using Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.UWO;
using Service.Interface;
using System.Security.Claims;

namespace API_UsePrevention.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;


        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _courseService.GetAllAsync();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var course = await _courseService.GetByIdAsync(id);
                return Ok(course);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _courseService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CourseRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedCourse = await _courseService.UpdateAsync(id, request);
                return Ok(updatedCourse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _courseService.DeleteAsync(id);
                return Ok(new { message = "Deleted successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("module/{id}")]
        public async Task<IActionResult> UpdateModule(int id, [FromBody] CourseModuleRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _courseService.UpdateModuleAsync(id, dto);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("lesson/{id}")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] CourseLessonRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _courseService.UpdateLessonAsync(id, dto);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("module/{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _courseService.DeleteModuleAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("lesson/{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _courseService.DeleteLessonAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{courseId}/module")]
        public async Task<IActionResult> AddModule(int courseId, [FromBody] CourseModuleRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _courseService.AddModuleAsync(courseId, dto);
                return Ok(new { message = "Module added successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("module/{moduleId}/lesson")]
        public async Task<IActionResult> AddLesson(int moduleId, [FromBody] CourseLessonRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _courseService.AddLessonAsync(moduleId, dto);
                return Ok(new { message = "Lesson added successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }



        [HttpPost("/ratings")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddRating(int courseId, [FromBody] CourseRatingRequestDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _courseService.AddRatingAsync(courseId, userId, dto);
            return Ok();
        }
    }
}
