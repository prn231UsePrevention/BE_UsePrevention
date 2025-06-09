using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class AnswerDto
    {
        public int QuestionId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
