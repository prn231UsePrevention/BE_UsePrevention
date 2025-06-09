using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dto.Request;
using Dto.Response;
using Repository.Models;
using Repository.UWO;
using Service.Interface;
using Service.Interface.Service.Interface;

namespace Service.Service
{
    public class AssessmentAnswerService : IAssessmentAnswerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssessmentAnswerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task AddAsync(AssessmentAnswerCreateDto dto)
        {
            var entity = new AssessmentAnswer
            {
                AssessmentResultId = dto.AssessmentResultId,
                QuestionId = dto.QuestionId,
                Value = dto.Value
            };
            await _unitOfWork.AssessmentAnswer.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var answer = await _unitOfWork.AssessmentAnswer.GetByIdAsync(id);
            if (answer != null)
            {
                _unitOfWork.AssessmentAnswer.DeleteAsync(answer);
                await _unitOfWork.CommitAsync();
            }
        }

    
        public async Task<IEnumerable<AssessmentAnswerDto>> GetAllByResultIdAsync(int assessmentResultId)
        {
            var entities = await _unitOfWork.AssessmentAnswer.FindAsync(v=>v.AssessmentResultId ==assessmentResultId);
            return entities.Select(e => new AssessmentAnswerDto
            {
                Id = e.Id,
                AssessmentResultId = e.AssessmentResultId,
                QuestionId = e.QuestionId,
                Value = e.Value
            });
        }


        public async Task<AssessmentAnswerDto?> GetByIdAsync(int id)
        {
            var e = await _unitOfWork.AssessmentAnswer.GetByIdAsync(id);
            if (e == null) return null;
            return new AssessmentAnswerDto
            {
                Id = e.Id,
                AssessmentResultId = e.AssessmentResultId,
                QuestionId = e.QuestionId,
                Value = e.Value
            };
        }

        public async Task UpdateAsync(int id, AssessmentAnswerUpdateDto dto)
        {
            var entity = await _unitOfWork.AssessmentAnswer.GetByIdAsync(id);
            if (entity == null) return;
            entity.Value = dto.Value;
            await _unitOfWork.CommitAsync();
        }
    }
}
