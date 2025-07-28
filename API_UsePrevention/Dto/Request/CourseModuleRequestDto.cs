using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class CourseModuleRequestDto
    {
        public string Title { get; set; }
        public List<CourseLessonRequestDto> Lessons { get; set; } = new();
    }
}
