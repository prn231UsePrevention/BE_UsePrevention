using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class ConsultantCreateDto
    {
        public int UserId { get; set; }

        public string Degree { get; set; }

        public string Specialty { get; set; }

        public string WorkSchedule { get; set; }
    }

    public class ConsultantUpdateDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Degree { get; set; }

        public string Specialty { get; set; }

        public string WorkSchedule { get; set; }
    }
}
