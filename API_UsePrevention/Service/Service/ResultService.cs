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

        public async Task<int> CreateResultAsync(CreateResultRequestDto request)
        {
            // Kiểm tra appointment hợp lệ
            var appointment = await _unitOfWork.Appointment.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");
            if (appointment.Status != "Completed")
                throw new InvalidOperationException("Only completed appointments can have results");

            // Kiểm tra đã tồn tại result cho appointment này chưa (nếu muốn giới hạn 1-1)
            var existing = (await _unitOfWork.Result.FindAsync(r => r.AppointmentId == request.AppointmentId && r.UserId == request.UserId)).FirstOrDefault();
            if (existing != null)
                throw new InvalidOperationException("Result for this appointment and user already exists");

            // Tạo mới result
            var result = new Repository.Models.Result
            {
                AppointmentId = request.AppointmentId,
                UserId = request.UserId,
                Diagnosis = request.Diagnosis,
                Recommendation = request.Recommendation,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _unitOfWork.Result.AddAsync(result);
            await _unitOfWork.CommitAsync();
            return result.Id;
        }
    }
}
