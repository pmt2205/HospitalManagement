using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QLBV.DAL.Entities;

namespace QLBV.DAL.Repositories
{
    public class PatientRepository
    {
        private readonly string _conn;

        public PatientRepository(string connectionString)
        {
            _conn = connectionString;
        }

        // Lấy patient theo UserId
        public Patient GetByUserId(int userId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT PatientId, UserId, DateOfBirth, Gender, Address
                    FROM Patient
                    WHERE UserId=@UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Patient
                        {
                            PatientId = (int)reader["PatientId"],
                            UserId = (int)reader["UserId"],
                            DateOfBirth = (DateTime)reader["DateOfBirth"],
                            Gender = reader["Gender"].ToString(),
                            Address = reader["Address"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        // Thêm patient mới
        public void Add(Patient patient)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    INSERT INTO Patient (UserId, DateOfBirth, Gender, Address)
                    VALUES (@UserId, @DateOfBirth, @Gender, @Address)", conn);

                cmd.Parameters.AddWithValue("@UserId", patient.UserId);
                cmd.Parameters.AddWithValue("@DateOfBirth", patient.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", patient.Gender ?? "");
                cmd.Parameters.AddWithValue("@Address", patient.Address ?? "");

                cmd.ExecuteNonQuery();
            }
        }
    }
}
