using Dto.Request;
using Dto.Request.Dto.Request;
using Dto.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IResultService
    {
        Task<ResultResponseDto> GetResultByIdAsync(int resultId);
        Task<IEnumerable<ResultResponseDto>> GetResultsByUserIdAsync(int userId);
        Task UpdateResultAsync(int resultId, UpdateResultRequestDto request);
        Task DeleteResultAsync(int resultId);
        Task<int> CreateResultAsync(CreateResultRequestDto request);

    }
}
