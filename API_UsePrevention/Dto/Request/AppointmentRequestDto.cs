using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class CreateSlotRequestDto
    {
        [Required]
        public DateTime StartTime { get; set; }
    }

    public class BookSlotRequestDto
    {
        [Required]
        public int SlotId { get; set; } // Là Appointment.Id
        public string Note { get; set; }
    }

    public class GetAvailableSlotsRequestDto
    {
        [Required]
        public int ConsultantId { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }

    public class UpdateAppointmentStatusRequestDto
    {
        [Required]
        public string Status { get; set; }
    }
}
