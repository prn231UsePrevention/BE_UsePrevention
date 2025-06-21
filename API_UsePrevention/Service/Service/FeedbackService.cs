using AutoMapper;
using Dto.Request;
using Repository.Models;
using Repository.UWO;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FeedbackDto>> GetAllAsync()
        {
            var feedbacks = await _unitOfWork.Feedback.GetAll();
            return _mapper.Map<IEnumerable<FeedbackDto>>(feedbacks);
        }

        public async Task<FeedbackDto?> GetByIdAsync(int id)
        {
            var feedback = await _unitOfWork.Feedback.GetByIdAsync(id);
            return feedback == null ? null : _mapper.Map<FeedbackDto>(feedback);
        }

        public async Task<FeedbackDto> CreateAsync(CreateFeedbackDto dto)
        {
            var feedback = _mapper.Map<Feedback>(dto);
            await _unitOfWork.Feedback.AddAsync(feedback);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<FeedbackDto>(feedback);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var feedback = await _unitOfWork.Feedback.GetByIdAsync(id);
            if (feedback == null) return false;

            await _unitOfWork.Feedback.DeleteAsync(feedback);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
