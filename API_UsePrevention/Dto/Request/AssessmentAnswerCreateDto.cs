using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class AssessmentAnswerCreateDto
    {
        public int AssessmentResultId { get; set; }
        public int QuestionId { get; set; }
        public string Value { get; set; }
    }
}
