using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class CourseRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> TargetAudience { get; set; } = new();
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? AdditionalInfo { get; set; }
        public bool? IsActive { get; set; }
        public string CourseGrade { get; set; }

        public List<CourseModuleRequestDto> Modules { get; set; } = new();
    }
}
