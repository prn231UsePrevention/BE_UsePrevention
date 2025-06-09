using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    using Dto.Request;
    using Dto.Response;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace Service.Interface
    {
        public interface IAssessmentAnswerService
        {
            Task<IEnumerable<AssessmentAnswerDto>> GetAllByResultIdAsync(int assessmentResultId);
            Task<AssessmentAnswerDto?> GetByIdAsync(int id);
            Task AddAsync(AssessmentAnswerCreateDto dto);
            Task UpdateAsync(int id, AssessmentAnswerUpdateDto dto);
            Task DeleteAsync(int id);
        }
    }

}
