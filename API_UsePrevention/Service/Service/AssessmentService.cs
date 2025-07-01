using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dto.Request;
using Repository.Models;
using Repository.UWO;
using Service.Interface;

namespace Service.Service
{
    public class AssessmentService : IAssessmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssessmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Assessment> CreateAssessmentAsync(AssessmentCreateDto dto)
        {
            var assessment = new Assessment
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Assessment.AddAsync(assessment);
            await _unitOfWork.CommitAsync();
            return assessment;
        }

        public async Task<bool> DeleteAssessmentAsync(int id)
        {
            var assessment = await _unitOfWork.Assessment.GetByIdAsync(id);
            if (assessment == null) return false;

            _unitOfWork.Assessment.DeleteAsync(assessment);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<IEnumerable<Assessment>> GetAllAssessmentsAsync()
        {
            return await _unitOfWork.Assessment.GetAll();
        }

        public async Task<Assessment?> GetAssessmentByIdAsync(int id)
        {
            return await _unitOfWork.Assessment.GetByIdAsync(id);
        }

        public async Task<AssessmentResult?> GetAssessmentResultByIdAsync(int resultId)
        {
            return await _unitOfWork.AssessmentResult.GetByIdAsync(resultId);
        }

        public async Task<IEnumerable<AssessmentResult>> GetUserAssessmentResultsAsync(int userId)
        {
            return await _unitOfWork.AssessmentResult.FindAsync(x => x.UserId == userId);
        }

        public async Task<AssessmentResult> SubmitAssessmentAsync(int userId, int assessmentId, List<AnswerDto> answers)
        {
            

            var questionIds = answers.Select(a => a.QuestionId).ToList();
            var questions = await _unitOfWork.AssessmentQuestion.FindAsync(q =>
                q.AssessmentId == assessmentId && questionIds.Contains(q.Id));

            int correct = 0;
            foreach (var answer in answers)
            {
                var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question != null && answer.Value?.Trim().Equals(question.CorrectAnswer?.Trim(), StringComparison.OrdinalIgnoreCase) == true)
                {
                    correct++;
                }
            }

            int total = questions.Count(); 
            double score10 = total == 0 ? 0 : (double)correct / total * 10;
            int score = (int)Math.Round(score10, 0); 


            string riskLevel = score >= 5 ? "Cao" : "Thấp";
            string suggestion = score >= 5 ? "Bạn nên đặt lịch tư vấn với chuyên viên." : "Bạn có thể tham gia khoá học phòng tránh.";

            var result = new AssessmentResult
            {
                UserId = userId,
                AssessmentId = assessmentId,
                Score = score,
                RiskLevel = riskLevel,
                Suggestion = suggestion,
                TakenAt = DateTime.Now
            };
            await _unitOfWork.AssessmentResult.AddAsync(result);
            await _unitOfWork.CommitAsync();

            foreach (var answer in answers)
            {
                var answerEntity = new AssessmentAnswer
                {
                    AssessmentResultId = result.Id,
                    QuestionId = answer.QuestionId,
                    Value = answer.Value
                };
                await _unitOfWork.AssessmentAnswer.AddAsync(answerEntity);
            }
            await _unitOfWork.CommitAsync();

            return result;
        }


        public async Task<bool> UpdateAssessmentAsync(int id, AssessmentUpdateDto dto)
        {
            var assessment = await _unitOfWork.Assessment.GetByIdAsync(id);
            if (assessment == null) return false;

            assessment.Name = dto.Name;
            assessment.Description = dto.Description;
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> DeleteAssessmentResultAsync(int resultId)
        {
            var result = await _unitOfWork.AssessmentResult.GetByIdAsync(resultId);
            if (result == null) return false;

            // Optionally, delete associated answers if they are not cascaded
            // var answers = await _unitOfWork.AssessmentAnswer.FindAsync(a => a.AssessmentResultId == resultId);
            // foreach (var answer in answers)
            // {
            //     _unitOfWork.AssessmentAnswer.DeleteAsync(answer);
            // }

            _unitOfWork.AssessmentResult.DeleteAsync(result);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
