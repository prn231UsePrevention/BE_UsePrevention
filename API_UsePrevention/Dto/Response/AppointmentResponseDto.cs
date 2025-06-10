using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Response
{
    public class ConsultantResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Degree { get; set; }
        public string Specialty { get; set; }
    }

    public class AvailableSlotResponseDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class AppointmentResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public int ConsultantId { get; set; }
        public string ConsultantFullName { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }
}
