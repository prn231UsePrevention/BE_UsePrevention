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
        public string TargetGroup { get; set; }
        public bool? IsActive { get; set; }
    }
}
