using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QLBV.DTO;

namespace QLBV.DAL.Repositories
{
    public class DoctorRepository
    {
        private readonly string _conn;
        public DoctorRepository(string connectionString) => _conn = connectionString;

        public List<DoctorDto> GetByDepartment(int departmentId)
        {
            var list = new List<DoctorDto>();
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT d.DoctorId, u.FullName, d.DepartmentId, dep.Name AS DepartmentName, d.ExtraFee
                    FROM Doctor d
                    JOIN [User] u ON d.UserId = u.UserId
                    JOIN Department dep ON d.DepartmentId = dep.DepartmentId
                    WHERE d.DepartmentId=@DepartmentId", conn);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DoctorDto
                        {
                            DoctorId = (int)reader["DoctorId"],
                            FullName = reader["FullName"].ToString(),
                            DepartmentId = (int)reader["DepartmentId"],
                            DepartmentName = reader["DepartmentName"].ToString(),
                            ExtraFee = (decimal)reader["ExtraFee"]
                        });
                    }
                }
            }
            return list;
        }

        public DoctorDto GetById(int doctorId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT d.DoctorId, u.FullName, d.DepartmentId, dep.Name AS DepartmentName, d.ExtraFee
                    FROM Doctor d
                    JOIN [User] u ON d.UserId = u.UserId
                    JOIN Department dep ON d.DepartmentId = dep.DepartmentId
                    WHERE d.DoctorId=@DoctorId", conn);
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
                            DepartmentName = reader["DepartmentName"].ToString(),
                            ExtraFee = (decimal)reader["ExtraFee"]
                        };
                    }
                }
            }
            return null;
        }

        public List<DoctorDto> GetAll()
        {
            var list = new List<DoctorDto>();
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT DoctorId, FullName FROM Doctor", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new DoctorDto
                            {
                                DoctorId = (int)reader["DoctorId"],
                                FullName = reader["FullName"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}
