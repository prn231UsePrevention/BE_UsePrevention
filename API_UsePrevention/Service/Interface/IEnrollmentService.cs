using Dto.Request;
using Dto.Response;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<EnrollmentResponseDto>> GetAllAsync();
        Task<EnrollmentResponseDto?> GetByIdAsync(int id);
        Task<EnrollmentResponseDto> CreateAsync(EnrollmentRequestDto dto);
        Task<bool> UpdateAsync(int id, EnrollmentRequestDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
