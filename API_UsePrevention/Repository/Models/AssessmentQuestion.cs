using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class AssessmentQuestion
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }        
        public string Content { get; set; }          
        public string Type { get; set; }             
        public string? Options { get; set; }         
        public string? CorrectAnswer { get; set; }   

        public virtual Assessment Assessment { get; set; }
        public virtual ICollection<AssessmentAnswer> AssessmentAnswers { get; set; }
    }

}
