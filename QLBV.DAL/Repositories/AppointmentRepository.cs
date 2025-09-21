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

        // Kiểm tra lịch trống
        public bool IsScheduleAvailable(int scheduleId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM Appointment
                    WHERE ScheduleId=@ScheduleId AND Status IN ('Pending','Confirmed')", conn))
                {
                    cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);
                    return (int)cmd.ExecuteScalar() == 0;
                }
            }
        }

        // Tạo appointment
        public int CreateAppointment(AppointmentDto dto)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    INSERT INTO Appointment (PatientId, DoctorId, DiseaseId, ScheduleId, AppointmentDate, Status)
                    VALUES (@PatientId, @DoctorId, @DiseaseId, @ScheduleId, @AppointmentDate, @Status);
                    SELECT SCOPE_IDENTITY();", conn))
                {
                    cmd.Parameters.AddWithValue("@PatientId", dto.PatientId);
                    cmd.Parameters.AddWithValue("@DoctorId", dto.DoctorId);
                    cmd.Parameters.AddWithValue("@DiseaseId", dto.DiseaseId);
                    cmd.Parameters.AddWithValue("@ScheduleId", dto.ScheduleId);
                    cmd.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);
                    cmd.Parameters.AddWithValue("@Status", dto.Status);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // Tạo invoice
        public void CreateInvoice(int appointmentId, decimal amount, string paymentMethod)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    INSERT INTO Invoice (AppointmentId, Amount, PaymentMethod)
                    VALUES (@AppointmentId, @Amount, @PaymentMethod)", conn))
                {
                    cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Cập nhật trạng thái
        public void UpdateStatus(int appointmentId, string status)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    UPDATE Appointment
                    SET Status = @Status
                    WHERE AppointmentId = @AppointmentId", conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- Lấy Appointment chi tiết ---
        public AppointmentDto GetById(int appointmentId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT AppointmentId, PatientId, DoctorId, DiseaseId, ScheduleId, AppointmentDate, Status
                    FROM Appointment
                    WHERE AppointmentId=@AppointmentId", conn))
                {
                    cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new AppointmentDto
                            {
                                AppointmentId = (int)reader["AppointmentId"],
                                PatientId = (int)reader["PatientId"],
                                DoctorId = (int)reader["DoctorId"],
                                DiseaseId = (int)reader["DiseaseId"],
                                ScheduleId = (int)reader["ScheduleId"],
                                AppointmentDate = (DateTime)reader["AppointmentDate"],
                                Status = reader["Status"].ToString()
                            };
                        }
                        return null;
                    }
                }
            }
        }

        // --- Lấy Patient theo Id ---
        public PatientDto GetPatientById(int patientId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT PatientId, FullName, Phone, Email
                    FROM Patient
                    WHERE PatientId=@PatientId", conn))
                {
                    cmd.Parameters.AddWithValue("@PatientId", patientId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PatientDto
                            {
                                PatientId = (int)reader["PatientId"]
                            };
                        }
                        return null;
                    }
                }
            }
        }

        // --- Lấy Doctor theo Id ---
        public DoctorDto GetDoctorById(int doctorId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT DoctorId, FullName, DepartmentId, ExtraFee
                    FROM Doctor
                    WHERE DoctorId=@DoctorId", conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new DoctorDto
                            {
                                DoctorId = (int)reader["DoctorId"],
                                FullName = reader["FullName"].ToString(),
                                DepartmentId = (int)reader["DepartmentId"],
                                ExtraFee = (decimal)reader["ExtraFee"]
                            };
                        }
                        return null;
                    }
                }
            }
        }

        // --- Lấy Department theo Id ---
        public DepartmentDto GetDepartmentById(int departmentId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT DepartmentId, Name, BaseFee
                    FROM Department
                    WHERE DepartmentId=@DepartmentId", conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new DepartmentDto
                            {
                                DepartmentId = (int)reader["DepartmentId"],
                                Name = reader["Name"].ToString(),
                                BaseFee = (decimal)reader["BaseFee"]
                            };
                        }
                        return null;
                    }
                }
            }
        }

        // --- Lấy Disease theo Id ---
        public DiseaseDto GetDiseaseById(int diseaseId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT DiseaseId, Name
                    FROM Disease
                    WHERE DiseaseId=@DiseaseId", conn))
                {
                    cmd.Parameters.AddWithValue("@DiseaseId", diseaseId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new DiseaseDto
                            {
                                DiseaseId = (int)reader["DiseaseId"],
                                Name = reader["Name"].ToString()
                            };
                        }
                        return null;
                    }
                }
            }
        }
    }
}
