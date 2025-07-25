using AutoMapper;
using Dto.Request.Dto.Request;
using Dto.Response;
using Repository.UWO;
using Service.Interface;
using Microsoft.EntityFrameworkCore;


namespace Service.Service
{
    public class ResultService : IResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ResultService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultResponseDto> GetResultByIdAsync(int resultId)
        {
            var result = await _unitOfWork.Result.GetByIdAsync(resultId);
            if (result == null)
                throw new KeyNotFoundException($"Result with ID {resultId} not found");
            return _mapper.Map<ResultResponseDto>(result);
        }

        public async Task<IEnumerable<ResultResponseDto>> GetResultsByUserIdAsync(int userId)
        {
            var results = await _unitOfWork.Result.FindAsync(r => r.UserId == userId);
            return _mapper.Map<IEnumerable<ResultResponseDto>>(results);
        }

        public async Task UpdateResultAsync(int resultId, UpdateResultRequestDto request)
        {
            var result = await _unitOfWork.Result.GetByIdAsync(resultId);
            if (result == null)
                throw new KeyNotFoundException($"Result with ID {resultId} not found");

            result.Diagnosis = request.Diagnosis;
            result.Recommendation = request.Recommendation;
            result.UpdatedAt = DateTime.Now;
            await _unitOfWork.Result.UpdateAsync(result);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteResultAsync(int resultId)
        {
            var result = await _unitOfWork.Result.GetByIdAsync(resultId);
            if (result == null)
                throw new KeyNotFoundException($"Result with ID {resultId} not found");

            await _unitOfWork.Result.DeleteAsync(result);
            await _unitOfWork.CommitAsync();
        }
    }
}
