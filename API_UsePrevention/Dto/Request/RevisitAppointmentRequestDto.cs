using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class RevisitAppointmentRequestDto
    {
        public int PreviousAppointmentId { get; set; }
        public DateTime NewTime { get; set; }
    }
}
