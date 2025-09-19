using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLBV.DAL.Entities
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int DiseaseId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } // Pending/Confirmed/Cancelled/Completed
    }
}
