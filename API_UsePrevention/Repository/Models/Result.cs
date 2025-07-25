using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public class Result
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int UserId { get; set; }

        public string Diagnosis { get; set; }

        public string Recommendation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
