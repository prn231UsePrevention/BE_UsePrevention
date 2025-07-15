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

namespace Service.Service
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EnrollmentResponseDto>> GetAllAsync()
        {
            var enrollments = await _unitOfWork.Enrollment
                .Query()
                .Include(e => e.User)
                .Include(e => e.Course)
                .ToListAsync();

            return enrollments.Select(e => new EnrollmentResponseDto
            {
                Id = e.Id,
                UserId = e.UserId,
                UserFullName = e.User?.FullName ?? "",
                CourseId = e.CourseId,
                CourseTitle = e.Course?.Title ?? "",
                Progress = e.Progress,
                EnrollDate = e.EnrollDate
            });
        }

        public async Task<EnrollmentResponseDto?> GetByIdAsync(int id)
        {
            var e = await _unitOfWork.Enrollment
                .Query()
                .Include(e => e.User)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (e == null) return null;

            return new EnrollmentResponseDto
            {
                Id = e.Id,
                UserId = e.UserId,
                UserFullName = e.User?.FullName ?? "",
                CourseId = e.CourseId,
                CourseTitle = e.Course?.Title ?? "",
                Progress = e.Progress,
                EnrollDate = e.EnrollDate
            };
        }

        public async Task<EnrollmentResponseDto> CreateAsync(EnrollmentRequestDto dto)
        {
            var enrollment = new Enrollment
            {
                UserId = dto.UserId,
                CourseId = dto.CourseId,
                EnrollDate = dto.EnrollDate
            };

            await _unitOfWork.Enrollment.AddAsync(enrollment);
            await _unitOfWork.CommitAsync();

            var user = await _unitOfWork.User.GetByIdAsync(dto.UserId);
            var course = await _unitOfWork.Course.GetByIdAsync(dto.CourseId);

            return new EnrollmentResponseDto
            {
                Id = enrollment.Id,
                UserId = dto.UserId,
                UserFullName = user?.FullName ?? "",
                CourseId = dto.CourseId,
                CourseTitle = course?.Title ?? "",
                Progress = 0,
                EnrollDate = dto.EnrollDate
            };
        }

        public async Task<bool> UpdateAsync(int id, EnrollmentRequestDto dto)
        {
            var enrollment = await _unitOfWork.Enrollment.GetByIdAsync(id);
            if (enrollment == null) return false;

            enrollment.UserId = dto.UserId;
            enrollment.CourseId = dto.CourseId;
            enrollment.Progress = dto.Progress;
            enrollment.EnrollDate = dto.EnrollDate;

            await _unitOfWork.Enrollment.UpdateAsync(enrollment);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var enrollment = await _unitOfWork.Enrollment.GetByIdAsync(id);
            if (enrollment == null) return false;

            await _unitOfWork.Enrollment.DeleteAsync(enrollment);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
