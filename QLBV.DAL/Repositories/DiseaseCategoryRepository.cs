using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QLBV.DTO;

namespace QLBV.DAL.Repositories
{
    public class DiseaseCategoryRepository
    {
        private readonly string _conn;
        public DiseaseCategoryRepository(string connectionString) => _conn = connectionString;

        // --- Lấy tất cả loại bệnh ---
        public List<DiseaseCategoryDto> GetAll()
        {
            var list = new List<DiseaseCategoryDto>();
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT DiseaseCategoryId, Name, Description, DepartmentId FROM DiseaseCategory", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new DiseaseCategoryDto
                            {
                                DiseaseCategoryId = (int)reader["DiseaseCategoryId"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                DepartmentId = (int)reader["DepartmentId"]
                            });
                        }
                    }
                }
            }
            return list;
        }

        // --- Lấy theo Id ---
        public DiseaseCategoryDto GetById(int id)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT DiseaseCategoryId, Name, Description, DepartmentId FROM DiseaseCategory WHERE DiseaseCategoryId=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new DiseaseCategoryDto
                            {
                                DiseaseCategoryId = (int)reader["DiseaseCategoryId"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                DepartmentId = (int)reader["DepartmentId"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        // --- Lấy theo DepartmentId ---
        public List<DiseaseCategoryDto> GetByDepartmentId(int departmentId)
        {
            var list = new List<DiseaseCategoryDto>();
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT DiseaseCategoryId, Name, Description, DepartmentId FROM DiseaseCategory WHERE DepartmentId=@DeptId", conn))
                {
                    cmd.Parameters.AddWithValue("@DeptId", departmentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new DiseaseCategoryDto
                            {
                                DiseaseCategoryId = (int)reader["DiseaseCategoryId"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                DepartmentId = (int)reader["DepartmentId"]
                            });
                        }
                    }
                }
            }
            return list;
        }

        // --- Thêm mới ---
        public int Add(DiseaseCategoryDto dto)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    INSERT INTO DiseaseCategory (Name, Description, DepartmentId)
                    VALUES (@Name, @Description, @DepartmentId);
                    SELECT SCOPE_IDENTITY();", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", dto.Name);
                    cmd.Parameters.AddWithValue("@Description", dto.Description ?? "");
                    cmd.Parameters.AddWithValue("@DepartmentId", dto.DepartmentId);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // --- Cập nhật ---
        public void Update(DiseaseCategoryDto dto)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    UPDATE DiseaseCategory
                    SET Name=@Name, Description=@Description, DepartmentId=@DepartmentId
                    WHERE DiseaseCategoryId=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", dto.Name);
                    cmd.Parameters.AddWithValue("@Description", dto.Description ?? "");
                    cmd.Parameters.AddWithValue("@DepartmentId", dto.DepartmentId);
                    cmd.Parameters.AddWithValue("@Id", dto.DiseaseCategoryId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- Xóa ---
        public void Delete(int id)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand("DELETE FROM DiseaseCategory WHERE DiseaseCategoryId=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
