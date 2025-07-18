﻿using Dto.Request;
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
                TargetAudience = c.TargetGroup?.Split(", ").ToList() ?? new(),
                Location = c.Location,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                ImageUrl = c.ImageUrl,
                AdditionalInfo = c.AdditionalInfo,
                CreatedAt = c.CreatedAt,
                IsActive = c.IsActive,
                CourseGrade = c.CourseGrade,
            });
        }

        public async Task<CourseResponseDto> GetByIdAsync(int id)
        {
            var course = await _unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException("Course not found");

            return new CourseResponseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                TargetAudience = course.TargetGroup?.Split(", ").ToList() ?? new(),
                Location = course.Location,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                ImageUrl = course.ImageUrl,
                AdditionalInfo = course.AdditionalInfo,
                CreatedAt = course.CreatedAt,
                IsActive = course.IsActive,
                CourseGrade = course.CourseGrade,
            };
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

            return new CourseResponseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                TargetAudience = request.TargetAudience,
                Location = course.Location,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                ImageUrl = course.ImageUrl,
                AdditionalInfo = course.AdditionalInfo,
                CreatedAt = course.CreatedAt,
                IsActive = course.IsActive,
                CourseGrade = course.CourseGrade,
            };
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

            return new CourseResponseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                TargetAudience = request.TargetAudience,
                Location = course.Location,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                ImageUrl = course.ImageUrl,
                AdditionalInfo = course.AdditionalInfo,
                CreatedAt = course.CreatedAt,
                IsActive = course.IsActive,
                CourseGrade = course.CourseGrade,
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

    }
}
