using Dto.Request;
using Repository.Models;

namespace Service.Interface
{
    public interface IAssessmentService
    {

        Task<IEnumerable<Assessment>> GetAllAssessmentsAsync();

        Task<Assessment?> GetAssessmentByIdAsync(int id);

        Task<AssessmentResult> SubmitAssessmentAsync(int userId, int assessmentId, List<AnswerDto> answers);

        Task<IEnumerable<AssessmentResult>> GetUserAssessmentResultsAsync(int userId);

        Task<AssessmentResult?> GetAssessmentResultByIdAsync(int resultId);
        Task<Assessment> CreateAssessmentAsync(AssessmentCreateDto dto);
        Task<bool> UpdateAssessmentAsync(int id, AssessmentUpdateDto dto);
        Task<bool> DeleteAssessmentAsync(int id);
    }
}
