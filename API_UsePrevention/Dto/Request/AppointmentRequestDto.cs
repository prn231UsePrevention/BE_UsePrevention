using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class CreateAppointmentRequestDto
    {
        public int ConsultantId { get; set; }
        public DateTime DateTime { get; set; }
        public string Note { get; set; }
    }

    public class UpdateAppointmentStatusRequestDto
    {
        public string Status { get; set; }
    }

    public class GetAvailableSlotsRequestDto
    {
        public int ConsultantId { get; set; }
        public DateTime Date { get; set; }
    }
}
