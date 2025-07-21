using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Response
{
    public class RevisitAppointmentResponseDto
    {
        public int Id { get; set; }
        public int ConsultantId { get; set; }
        public int? UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public bool IsRevisit { get; set; }      
        public int? ParentAppointmentId { get; set; } 
        public string UserFullName { get; set; }
        public string ConsultantFullName { get; set; }
    }
}
