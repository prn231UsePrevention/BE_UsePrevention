using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dto.Request;

namespace Service.Interface
{
    public interface IAssessmentQuestionService
    {
        Task<IEnumerable<AssessmentQuestionDto>> GetAllAsync();

        Task<AssessmentQuestionDto?> GetByIdAsync(int id);

        Task<IEnumerable<AssessmentQuestionDto>> GetByAssessmentIdAsync(int assessmentId);

        Task AddAsync(AssessmentQuestionCreateDto questionCreateDto);

        Task UpdateAsync(int id, AssessmentQuestionUpdateDto questionUpdateDto);

        Task DeleteAsync(int id);
    }
}
