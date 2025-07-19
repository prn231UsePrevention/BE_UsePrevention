using Dto.Request;
using Dto.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseResponseDto>> GetAllAsync();
        Task<CourseResponseDto> GetByIdAsync(int id);
        Task<CourseResponseDto> CreateAsync(CourseRequestDto request);
        Task<CourseResponseDto> UpdateAsync(int id, CourseRequestDto request);
        Task DeleteAsync(int id);
        Task<IEnumerable<CourseResponseDto>> GetCoursesByUserIdAsync(int userId);
    }
}
