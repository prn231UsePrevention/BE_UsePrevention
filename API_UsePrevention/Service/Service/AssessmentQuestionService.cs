using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dto.Request;
using Dto.Response;
using Repository.Models;
using Repository.UWO;
using Service.Interface;

namespace Service.Service
{
    public class AssessmentQuestionService : IAssessmentQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AssessmentQuestionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(AssessmentQuestionCreateDto dto)
        {
            var entity = new AssessmentQuestion
            {
                AssessmentId = dto.AssessmentId,
                Content = dto.Content,
                Type = dto.Type,
                Options = dto.Options,
                CorrectAnswer = dto.CorrectAnswer
            };
            await _unitOfWork.AssessmentQuestion.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var question = await _unitOfWork.AssessmentQuestion.GetByIdAsync(id);
            if (question != null)
            {
                _unitOfWork.AssessmentQuestion.DeleteAsync(question);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task<IEnumerable<AssessmentQuestionDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.AssessmentQuestion.GetAll();
            return entities.Select(e => new AssessmentQuestionDto
            {
                Id = e.Id,
                AssessmentId = e.AssessmentId,
                Content = e.Content,
                Type = e.Type,
                Options = e.Options
            });
        }

     
        public async Task<IEnumerable<AssessmentQuestionDto>> GetByAssessmentIdAsync(int assessmentId)
        {
            var entities = await _unitOfWork.AssessmentQuestion.FindAsync(v => v.AssessmentId==assessmentId);
            return entities.Select(e => new AssessmentQuestionDto
            {
                Id = e.Id,
                AssessmentId = e.AssessmentId,
                Content = e.Content,
                Type = e.Type,
                Options = e.Options
            });
        }

        public async Task<AssessmentQuestionDto?> GetByIdAsync(int id)
        {
            var e = await _unitOfWork.AssessmentQuestion.GetByIdAsync(id);
            if (e == null) return null;
            return new AssessmentQuestionDto
            {
                Id = e.Id,
                AssessmentId = e.AssessmentId,
                Content = e.Content,
                Type = e.Type,
                Options = e.Options
            };
        }

        public async Task UpdateAsync(int id, AssessmentQuestionUpdateDto dto)
        {
            var entity = await _unitOfWork.AssessmentQuestion.GetByIdAsync(id);
            if (entity == null) return;
            entity.Content = dto.Content;
            entity.Type = dto.Type;
            entity.Options = dto.Options;
            entity.CorrectAnswer = dto.CorrectAnswer;
            await _unitOfWork.CommitAsync();
        }
    }
}
