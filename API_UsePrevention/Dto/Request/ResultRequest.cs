using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    namespace Dto.Request
    {
        public class UpdateResultRequestDto
        {
            public string Diagnosis { get; set; }
            public string Recommendation { get; set; }
        }

        public class CreateResultRequestDto
        {
            public int AppointmentId { get; set; }
            public int UserId { get; set; }         
            public string Diagnosis { get; set; }
            public string Recommendation { get; set; }
        }
    }
}
