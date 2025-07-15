using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class EnrollmentRequestDto
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int? Progress { get; set; }
        public DateTime? EnrollDate { get; set; }
    }
}
