using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLBV.DAL.Repositories;
using QLBV.DTO;

namespace QLBV.BLL
{
    public class ScheduleService
    {
        private readonly ScheduleRepository _scheduleRepository;

        public ScheduleService(ScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public List<ScheduleDto> GetSchedulesByDoctor(int doctorId)
        {
            return _scheduleRepository.GetSchedulesByDoctor(doctorId);
        }

        public bool IsDoctorAvailable(int doctorId, int scheduleId)
        {
            return _scheduleRepository.IsDoctorAvailable(doctorId, scheduleId);
        }
    }
}
