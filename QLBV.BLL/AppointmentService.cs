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
        private readonly PatientRepository _patientRepo; // thêm repo Patient nếu có
        private readonly DiseaseRepository _diseaseRepo; // Thêm


        public AppointmentService(
            AppointmentRepository appointmentRepo,
            ScheduleRepository scheduleRepo,
            DoctorRepository doctorRepo,
            DepartmentRepository departmentRepo,
            PatientRepository patientRepo,
            DiseaseRepository diseaseRepo) // thêm

        {
            _appointmentRepo = appointmentRepo;
            _scheduleRepo = scheduleRepo;
            _doctorRepo = doctorRepo;
            _departmentRepo = departmentRepo;
            _patientRepo = patientRepo;
            _diseaseRepo = diseaseRepo;// thêm
        }

        public bool CheckScheduleAvailable(int scheduleId)
        {
            return _appointmentRepo.IsScheduleAvailable(scheduleId);
        }

        public int BookAppointment(AppointmentDto dto, string paymentMethod)
        {
            var doctor = _doctorRepo.GetById(dto.DoctorId);
            var department = _departmentRepo.GetById(doctor.DepartmentId);

            decimal amount = department.BaseFee + doctor.ExtraFee;

            int appointmentId = _appointmentRepo.CreateAppointment(dto);

            _appointmentRepo.CreateInvoice(appointmentId, amount, paymentMethod);

            return appointmentId;
        }

        public List<ScheduleDto> GetSchedulesByDoctor(int doctorId)
        {
            return _scheduleRepo.GetSchedulesByDoctor(doctorId);
        }

        public void MarkPaid(int appointmentId, string paymentMethod)
        {
            _appointmentRepo.UpdateStatus(appointmentId, "Confirmed");
        }

        public ScheduleDto GetScheduleById(int scheduleId)
        {
            return _scheduleRepo.GetScheduleById(scheduleId);
        }

        // --- Bổ sung các phương thức để lấy chi tiết ---
        public AppointmentDto GetAppointmentById(int appointmentId)
        {
            return _appointmentRepo.GetById(appointmentId);
        }

        public PatientDto GetPatientById(int patientId)
        {
            return _patientRepo.GetById(patientId);
        }

        public DoctorDto GetDoctorById(int doctorId)
        {
            return _doctorRepo.GetById(doctorId);
        }

        public DepartmentDto GetDepartmentById(int departmentId)
        {
            return _departmentRepo.GetById(departmentId);
        }
        public DiseaseDto GetDiseaseById(int diseaseId)
        {
            return _diseaseRepo.GetById(diseaseId);
        }
    }
}
