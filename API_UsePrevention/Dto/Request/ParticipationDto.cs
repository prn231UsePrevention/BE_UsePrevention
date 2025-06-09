using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class ParticipationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProgramId { get; set; }
        public string PreSurvey { get; set; }
        public string PostSurvey { get; set; }
        public DateTime? JoinedAt { get; set; }
    }


    public class ParticipationCreateUpdateDto
    {
        public int ProgramId { get; set; }
        public string PreSurvey { get; set; }
        public string PostSurvey { get; set; }
        public DateTime? JoinedAt { get; set; }
    }
}
