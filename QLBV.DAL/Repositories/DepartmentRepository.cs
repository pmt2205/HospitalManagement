using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QLBV.DTO;

namespace QLBV.DAL.Repositories
{
    public class DepartmentRepository
    {
        private readonly string _conn;
        public DepartmentRepository(string connectionString) => _conn = connectionString;

        // --- Lấy tất cả khoa ---
        public List<DepartmentDto> GetAll()
        {
            var list = new List<DepartmentDto>();
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT DepartmentId, Name, Description, BaseFee FROM Department", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new DepartmentDto
                            {
                                DepartmentId = (int)reader["DepartmentId"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                BaseFee = (decimal)reader["BaseFee"]
                            });
                        }
                    }
                }
            }
            return list;
        }

        // --- Lấy khoa theo Id ---
        public DepartmentDto GetById(int id)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT DepartmentId, Name, Description, BaseFee FROM Department WHERE DepartmentId=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new DepartmentDto
                            {
                                DepartmentId = (int)reader["DepartmentId"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                BaseFee = (decimal)reader["BaseFee"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        // --- Thêm mới khoa ---
        public int Add(DepartmentDto dept)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    INSERT INTO Department (Name, Description, BaseFee)
                    VALUES (@Name, @Description, @BaseFee);
                    SELECT SCOPE_IDENTITY();", conn))
                {
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = dept.Name ?? "";
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = dept.Description ?? "";
                    cmd.Parameters.Add("@BaseFee", SqlDbType.Decimal).Value = dept.BaseFee;


                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // --- Cập nhật khoa ---
        public void Update(DepartmentDto dept)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    UPDATE Department
                    SET Name=@Name, Description=@Description, BaseFee=@BaseFee
                    WHERE DepartmentId=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", dept.Name);
                    cmd.Parameters.AddWithValue("@Description", dept.Description ?? "");
                    cmd.Parameters.AddWithValue("@BaseFee", dept.BaseFee);
                    cmd.Parameters.AddWithValue("@Id", dept.DepartmentId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- Xóa khoa ---
        public void Delete(int id)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand("DELETE FROM Department WHERE DepartmentId=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
