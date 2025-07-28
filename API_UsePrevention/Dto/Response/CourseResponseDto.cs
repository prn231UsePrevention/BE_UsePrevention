using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Response
{
    public class CourseResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> TargetAudience { get; set; } // convert from string
        public string Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ImageUrl { get; set; }
        public string AdditionalInfo { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }
        public string CourseGrade { get; set; }
        public List<CourseModuleResponseDto> Modules { get; set; }
        public List<CourseRatingResponseDto> Ratings { get; set; }
    }
}
