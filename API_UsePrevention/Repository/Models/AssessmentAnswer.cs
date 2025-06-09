using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class AssessmentAnswer
    {
        public int Id { get; set; }
        public int AssessmentResultId { get; set; }
        public int QuestionId { get; set; }
        public string Value { get; set; }

        public virtual AssessmentResult AssessmentResult { get; set; }
        public virtual AssessmentQuestion Question { get; set; }
    }
}
