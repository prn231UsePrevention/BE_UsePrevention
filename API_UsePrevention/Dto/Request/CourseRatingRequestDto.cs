using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class CourseRatingRequestDto
    {
        public int Stars { get; set; }
        public string? Comment { get; set; }
    }
}
