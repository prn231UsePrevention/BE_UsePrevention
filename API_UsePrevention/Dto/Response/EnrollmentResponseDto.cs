using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Response
{
    public class EnrollmentResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int? Progress { get; set; }
        public DateTime? EnrollDate { get; set; }
    }
}
