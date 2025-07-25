using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Response
{
    public class ResultResponseDto
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int UserId { get; set; }
        public string Diagnosis { get; set; }
        public string Recommendation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
