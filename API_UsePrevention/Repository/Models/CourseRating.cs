using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Models
{
    public partial class CourseRating
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public int UserId { get; set; }

        public int Stars { get; set; } // 1–5
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Course Course { get; set; }
        public virtual User User { get; set; }
    }
}
