using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class FeedbackDto
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int ConsultantId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? AssessmentSummary { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    public class CreateFeedbackDto
    {
        public int AppointmentId { get; set; }
        public int ConsultantId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? AssessmentSummary { get; set; }
    }

    public class UpdateFeedbackDto
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? AssessmentSummary { get; set; }
    }
}
