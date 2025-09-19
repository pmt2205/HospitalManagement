using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBV.DAL.Entities
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public string Specialization { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal ExtraFee { get; set; }
    }
}
