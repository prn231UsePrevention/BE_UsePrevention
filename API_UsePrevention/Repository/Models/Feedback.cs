using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int ConsultantId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? AssessmentSummary { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Appointment Appointment { get; set; }
        public virtual Consultant Consultant { get; set; }
        public virtual User User { get; set; }
    }

}
