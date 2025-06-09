using Dto.Request;
using Dto.Response;
using Repository.Models;
using Repository.Repositories;
using Repository.UWO;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class CourseService :ICourseService
    {
        
        private readonly IUnitOfWork _unitOfWork;
        public CourseService( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CourseResponseDto>> GetAllAsync()
        {
            var courses = await _unitOfWork.Course.GetAll();
            return courses.Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                TargetGroup = c.TargetGroup,
                CreatedAt = c.CreatedAt,
                IsActive = c.IsActive
            });
        }

        public async Task<CourseResponseDto> GetByIdAsync(int id)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
            {
                throw new KeyNotFoundException("Course not found");
            }
            return new CourseResponseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                TargetGroup = course.TargetGroup,
                CreatedAt = course.CreatedAt,
                IsActive = course.IsActive
            };
        }

        public async Task<CourseResponseDto> CreateAsync(CourseRequestDto request)
        {
            var course = new Course
            {
                Title = request.Title,
                Description = request.Description,
                TargetGroup = request.TargetGroup,
                CreatedAt = DateTime.UtcNow,
                IsActive = request.IsActive ?? true
            };

            await _unitOfWork.Course.AddAsync(course);
            await _unitOfWork.CommitAsync();

            return new CourseResponseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                TargetGroup = course.TargetGroup,
                CreatedAt = course.CreatedAt,
                IsActive = course.IsActive
            };
        }

        public async Task<CourseResponseDto> UpdateAsync(int id, CourseRequestDto request)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException("Course not found");

            course.Title = request.Title;
            course.Description = request.Description;
            course.TargetGroup = request.TargetGroup;
            course.IsActive = request.IsActive;

            await _unitOfWork.Course.UpdateAsync(course);
            await _unitOfWork.CommitAsync();

            return new CourseResponseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                TargetGroup = course.TargetGroup,
                CreatedAt = course.CreatedAt,
                IsActive = course.IsActive
            };
        }

        public async Task DeleteAsync(int id)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException("Course not found");

            await _unitOfWork.Course.DeleteAsync(course);
            await _unitOfWork.CommitAsync();
        }
    }
}
