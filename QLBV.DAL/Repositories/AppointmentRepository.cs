using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QLBV.DTO;

namespace QLBV.DAL.Repositories
{
    public class AppointmentRepository
    {
        private readonly string _conn;
        public AppointmentRepository(string connectionString) => _conn = connectionString;

        public bool IsScheduleAvailable(int scheduleId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM Appointment
                    WHERE ScheduleId=@ScheduleId AND Status IN ('Pending','Confirmed')", conn);
                cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);
                return (int)cmd.ExecuteScalar() == 0;
            }
        }

        public int CreateAppointment(AppointmentDto dto)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    INSERT INTO Appointment (PatientId, DoctorId, DiseaseId, ScheduleId, AppointmentDate, Status)
                    VALUES (@PatientId, @DoctorId, @DiseaseId, @ScheduleId, @AppointmentDate, @Status);
                    SELECT SCOPE_IDENTITY();", conn);
                cmd.Parameters.AddWithValue("@PatientId", dto.PatientId);
                cmd.Parameters.AddWithValue("@DoctorId", dto.DoctorId);
                cmd.Parameters.AddWithValue("@DiseaseId", dto.DiseaseId);
                cmd.Parameters.AddWithValue("@ScheduleId", dto.ScheduleId);
                cmd.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);
                cmd.Parameters.AddWithValue("@Status", dto.Status);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void CreateInvoice(int appointmentId, decimal amount, string paymentMethod)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    INSERT INTO Invoice (AppointmentId, Amount, PaymentMethod)
                    VALUES (@AppointmentId, @Amount, @PaymentMethod)", conn);
                cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateStatus(int appointmentId, string status)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    UPDATE Appointment
                    SET Status = @Status
                    WHERE AppointmentId = @AppointmentId
                ", conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
