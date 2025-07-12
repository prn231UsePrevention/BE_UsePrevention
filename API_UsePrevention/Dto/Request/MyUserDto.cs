using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class MyUserDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }
        public string Role { get; set; }
    }
}
