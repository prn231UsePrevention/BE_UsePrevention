using Dto.Request;
using Dto.Response;
using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Repository.Repositories;
using Repository.UWO;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
namespace Service.Service
{
    public class CourseService :ICourseService
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CourseService( IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CourseResponseDto>> GetAllAsync()
        {
            var courses = await _unitOfWork.Course
            .Query()
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .Include(c => c.Ratings)
            .ToListAsync();

            return _mapper.Map<IEnumerable<CourseResponseDto>>(courses);
        }

        public async Task<CourseResponseDto> GetByIdAsync(int id)
        {
            var course = await _unitOfWork.Course
           .Query()
           .Include(c => c.Modules)
               .ThenInclude(m => m.Lessons)
           .Include(c => c.Ratings)
           .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                throw new KeyNotFoundException("Course not found");
            
            return _mapper.Map<CourseResponseDto>(course);

        }

        public async Task<CourseResponseDto> CreateAsync(CourseRequestDto request)
        {
            var course = new Course
            {
                Title = request.Title,
                Description = request.Description,
                TargetGroup = string.Join(", ", request.TargetAudience),
                Location = request.Location,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl)
                    ? "https://via.placeholder.com/300x200?text=Khóa+học"
                    : request.ImageUrl,
                AdditionalInfo = request.AdditionalInfo,
                CreatedAt = DateTime.UtcNow,
                IsActive = request.IsActive ?? true,
                CourseGrade = request.CourseGrade,
            };

            await _unitOfWork.Course.AddAsync(course);
            await _unitOfWork.CommitAsync();

            // ✅ Tạo module và lesson
            foreach (var moduleDto in request.Modules)
            {
                var module = new CourseModule
                {
                    CourseId = course.Id,
                    Title = moduleDto.Title
                };

                await _unitOfWork.CourseModule.AddAsync(module);
                await _unitOfWork.CommitAsync();

                foreach (var lessonDto in moduleDto.Lessons)
                {
                    var lesson = new CourseLesson
                    {
                        ModuleId = module.Id,
                        Title = lessonDto.Title,
                        Description = lessonDto.Description,
                        VideoUrl = lessonDto.VideoUrl,
                        Order = lessonDto.Order
                    };
                    await _unitOfWork.CourseLesson.AddAsync(lesson);
                }

                await _unitOfWork.CommitAsync();
            }

            return _mapper.Map<CourseResponseDto>(course);
        }

        public async Task<CourseResponseDto> UpdateAsync(int id, CourseRequestDto request)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException("Course not found");

            course.Title = request.Title;
            course.Description = request.Description;
            course.TargetGroup = string.Join(", ", request.TargetAudience);
            course.Location = request.Location;
            course.StartDate = request.StartDate;
            course.EndDate = request.EndDate;
            course.ImageUrl = request.ImageUrl;
            course.AdditionalInfo = request.AdditionalInfo;
            course.IsActive = request.IsActive;
            course.CourseGrade = request.CourseGrade;

            await _unitOfWork.Course.UpdateAsync(course);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<CourseResponseDto>(course);
        }

        public async Task DeleteAsync(int id)
        {
            var course = await _unitOfWork.Course
        .Query()
        .Include(c => c.Modules)
            .ThenInclude(m => m.Lessons)
        .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                throw new KeyNotFoundException("Course not found");

            foreach (var module in course.Modules)
            {
                foreach (var lesson in module.Lessons)
                {
                    await _unitOfWork.CourseLesson.DeleteAsync(lesson);
                }

                await _unitOfWork.CourseModule.DeleteAsync(module);
            }

            await _unitOfWork.Course.DeleteAsync(course);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<CourseResponseDto>> GetCoursesByUserIdAsync(int userId)
        {
            var enrollments = await _unitOfWork.Enrollment
                .Query()
                .Include(e => e.Course)
                .Where(e => e.UserId == userId)
                .ToListAsync();

            return enrollments
                .Where(e => e.Course != null)
                .Select(e => new CourseResponseDto
                {
                    Id = e.Course.Id,
                    Title = e.Course.Title,
                    Description = e.Course.Description,
                    Location = e.Course.Location,
                    StartDate = e.Course.StartDate,
                    EndDate = e.Course.EndDate,
                    ImageUrl = e.Course.ImageUrl,
                    AdditionalInfo = e.Course.AdditionalInfo
                });
        }

        public async Task AddRatingAsync(int courseId, int userId, CourseRatingRequestDto dto)
        {
            var exists = await _unitOfWork.CourseRating
                .Query()
                .AnyAsync(r => r.CourseId == courseId && r.UserId == userId);

            if (exists)
                throw new Exception("Bạn đã đánh giá khóa học này.");

            var rating = new CourseRating
            {
                CourseId = courseId,
                UserId = userId,
                Stars = dto.Stars,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CourseRating.AddAsync(rating);
            await _unitOfWork.CommitAsync();
        }


        public async Task UpdateModuleAsync(int moduleId, CourseModuleRequestDto dto)
        {
            var module = await _unitOfWork.CourseModule.GetByIdAsync(moduleId);
            if (module == null)
                throw new KeyNotFoundException("Module not found");

            module.Title = dto.Title;
            await _unitOfWork.CourseModule.UpdateAsync(module);
            await _unitOfWork.CommitAsync();
        }


        public async Task UpdateLessonAsync(int lessonId, CourseLessonRequestDto dto)
        {
            var lesson = await _unitOfWork.CourseLesson.GetByIdAsync(lessonId);
            if (lesson == null)
                throw new KeyNotFoundException("Lesson not found");

            lesson.Title = dto.Title;
            lesson.Description = dto.Description;
            lesson.VideoUrl = dto.VideoUrl;
            lesson.Order = dto.Order;

            await _unitOfWork.CourseLesson.UpdateAsync(lesson);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteModuleAsync(int moduleId)
        {
            var module = await _unitOfWork.CourseModule.GetByIdAsync(moduleId);
            if (module == null)
                throw new KeyNotFoundException("Module not found");

            await _unitOfWork.CourseModule.DeleteAsync(module);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteLessonAsync(int lessonId)
        {
            var lesson = await _unitOfWork.CourseLesson.GetByIdAsync(lessonId);
            if (lesson == null)
                throw new KeyNotFoundException("Lesson not found");

            await _unitOfWork.CourseLesson.DeleteAsync(lesson);
            await _unitOfWork.CommitAsync();
        }

        public async Task AddModuleAsync(int courseId, CourseModuleRequestDto dto)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(courseId);
            if (course == null)
                throw new KeyNotFoundException("Course not found");

            var module = new CourseModule
            {
                CourseId = courseId,
                Title = dto.Title
            };

            await _unitOfWork.CourseModule.AddAsync(module);
            await _unitOfWork.CommitAsync();
        }

        public async Task AddLessonAsync(int moduleId, CourseLessonRequestDto dto)
        {
            var module = await _unitOfWork.CourseModule.GetByIdAsync(moduleId);
            if (module == null)
                throw new KeyNotFoundException("Module not found");

            var lesson = new CourseLesson
            {
                ModuleId = moduleId,
                Title = dto.Title,
                Description = dto.Description,
                VideoUrl = dto.VideoUrl,
                Order = dto.Order
            };

            await _unitOfWork.CourseLesson.AddAsync(lesson);
            await _unitOfWork.CommitAsync();
        }




    }
}
