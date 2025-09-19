using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBV.DTO
{
    public class PatientDto
    {
        public int PatientId { get; set; }       // Khóa chính
        public int UserId { get; set; }          // Khóa ngoại tới User
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
    }
}
