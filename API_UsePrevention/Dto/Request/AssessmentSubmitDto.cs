using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class AssessmentSubmitDto
    {
       
        public int AssessmentId { get; set; }   
        public int UserId { get; set; }
        public List<AnswerDto> Answers { get; set; }
    }
}
