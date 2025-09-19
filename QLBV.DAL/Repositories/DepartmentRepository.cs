using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QLBV.DTO;

namespace QLBV.DAL.Repositories
{
    public class DepartmentRepository
    {
        private readonly string _conn;
        public DepartmentRepository(string connectionString) => _conn = connectionString;

        // Lấy tất cả khoa
        public List<DepartmentDto> GetAll()
        {
            var list = new List<DepartmentDto>();
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT DepartmentId, Name, BaseFee FROM Department", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DepartmentDto
                        {
                            DepartmentId = (int)reader["DepartmentId"],
                            Name = reader["Name"].ToString(),
                            BaseFee = (decimal)reader["BaseFee"]
                        });
                    }
                }
            }
            return list;
        }

        // Lấy thông tin khoa theo Id
        public DepartmentDto GetById(int id)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT DepartmentId, Name, BaseFee FROM Department WHERE DepartmentId=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);

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
                }
            }
            return null;
        }
    }
}
