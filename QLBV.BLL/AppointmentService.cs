using System;
using System.Collections.Generic;
using QLBV.DAL.Repositories;
using QLBV.DTO;

namespace QLBV.BLL
{
    public class AppointmentService
    {
        private readonly AppointmentRepository _appointmentRepo;
        private readonly ScheduleRepository _scheduleRepo;
        private readonly DoctorRepository _doctorRepo;
        private readonly DepartmentRepository _departmentRepo;

        public AppointmentService(
            AppointmentRepository appointmentRepo,
            ScheduleRepository scheduleRepo,
            DoctorRepository doctorRepo,
            DepartmentRepository departmentRepo)
        {
            _appointmentRepo = appointmentRepo;
            _scheduleRepo = scheduleRepo;
            _doctorRepo = doctorRepo;
            _departmentRepo = departmentRepo;
        }

        public bool CheckScheduleAvailable(int scheduleId)
        {
            return _appointmentRepo.IsScheduleAvailable(scheduleId);
        }

        public int BookAppointment(AppointmentDto dto, string paymentMethod)
        {
            // Lấy doctor để tính phí
            var doctor = _doctorRepo.GetById(dto.DoctorId);
            var department = _departmentRepo.GetById(doctor.DepartmentId);

            // Tổng phí = phí khoa + phụ thu bác sĩ
            decimal amount = department.BaseFee + doctor.ExtraFee;

            // Tạo appointment
            int appointmentId = _appointmentRepo.CreateAppointment(dto);

            // Tạo invoice
            _appointmentRepo.CreateInvoice(appointmentId, amount, paymentMethod);

            return appointmentId;
        }

        public List<ScheduleDto> GetSchedulesByDoctor(int doctorId)
        {
            return _scheduleRepo.GetSchedulesByDoctor(doctorId);
        }

        public void MarkPaid(int appointmentId, string paymentMethod)
        {
            // Muốn lấy amount chính xác thì lấy từ DB invoice hoặc tính lại như trên
            // Ở đây giả sử invoice đã tạo từ BookAppointment
            _appointmentRepo.UpdateStatus(appointmentId, "Confirmed");
        }

        public ScheduleDto GetScheduleById(int scheduleId)
        {
            return _scheduleRepo.GetScheduleById(scheduleId);
        }
    }
}
