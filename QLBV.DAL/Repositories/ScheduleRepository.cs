using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QLBV.DTO;

namespace QLBV.DAL.Repositories
{
    public class ScheduleRepository
    {
        private readonly string _conn;
        public ScheduleRepository(string connectionString) => _conn = connectionString;

        public List<ScheduleDto> GetSchedulesByDoctor(int doctorId)
        {
            var list = new List<ScheduleDto>();
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT ScheduleId, WorkDate, StartTime, EndTime
                    FROM Schedule
                    WHERE DoctorId=@DoctorId", conn);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ScheduleDto
                        {
                            ScheduleId = (int)reader["ScheduleId"],
                            DoctorId = doctorId,
                            WorkDate = (DateTime)reader["WorkDate"],
                            StartTime = (TimeSpan)reader["StartTime"],
                            EndTime = (TimeSpan)reader["EndTime"]
                        });
                    }
                }
            }
            return list;
        }
        public bool IsDoctorAvailable(int doctorId, int scheduleId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                SELECT COUNT(*) FROM Appointment
                WHERE DoctorId=@DoctorId AND ScheduleId=@ScheduleId AND Status IN ('Pending','Confirmed')", conn);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);
                return (int)cmd.ExecuteScalar() == 0;
            }
        }

        public ScheduleDto GetScheduleById(int scheduleId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
            SELECT ScheduleId, DoctorId, WorkDate, StartTime, EndTime
            FROM Schedule
            WHERE ScheduleId=@ScheduleId", conn);
                cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ScheduleDto
                        {
                            ScheduleId = (int)reader["ScheduleId"],
                            DoctorId = (int)reader["DoctorId"],
                            WorkDate = (DateTime)reader["WorkDate"],
                            StartTime = (TimeSpan)reader["StartTime"],
                            EndTime = (TimeSpan)reader["EndTime"]
                        };
                    }
                }
            }
            return null;
        }

    }
}
