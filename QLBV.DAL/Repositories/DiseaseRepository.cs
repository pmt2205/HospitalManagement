using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QLBV.DTO;

namespace QLBV.DAL.Repositories
{
    public class DiseaseRepository
    {
        private readonly string _conn;
        public DiseaseRepository(string connectionString) => _conn = connectionString;

        // --- Lấy tất cả bệnh ---
        public List<DiseaseDto> GetAll()
        {
            var list = new List<DiseaseDto>();
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT DiseaseId, CategoryId, Name, Description FROM Disease", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new DiseaseDto
                            {
                                DiseaseId = (int)reader["DiseaseId"],
                                CategoryId = (int)reader["CategoryId"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        // --- Lấy theo Id ---
        public DiseaseDto GetById(int diseaseId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT DiseaseId, CategoryId, Name, Description
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
                                CategoryId = (int)reader["CategoryId"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        // --- Lấy theo CategoryId ---
        public List<DiseaseDto> GetByCategory(int categoryId)
        {
            var list = new List<DiseaseDto>();
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT DiseaseId, Name, Description FROM Disease WHERE CategoryId=@CategoryId", conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new DiseaseDto
                            {
                                DiseaseId = (int)reader["DiseaseId"],
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                CategoryId = categoryId
                            });
                        }
                    }
                }
            }
            return list;
        }

        // --- Thêm mới ---
        public int Add(DiseaseDto dto)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    INSERT INTO Disease (CategoryId, Name, Description)
                    VALUES (@CategoryId, @Name, @Description);
                    SELECT SCOPE_IDENTITY();", conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryId", dto.CategoryId);
                    cmd.Parameters.AddWithValue("@Name", dto.Name);
                    cmd.Parameters.AddWithValue("@Description", dto.Description ?? "");

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // --- Cập nhật ---
        public void Update(DiseaseDto dto)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    UPDATE Disease
                    SET CategoryId=@CategoryId, Name=@Name, Description=@Description
                    WHERE DiseaseId=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryId", dto.CategoryId);
                    cmd.Parameters.AddWithValue("@Name", dto.Name);
                    cmd.Parameters.AddWithValue("@Description", dto.Description ?? "");
                    cmd.Parameters.AddWithValue("@Id", dto.DiseaseId);

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
                using (var cmd = new SqlCommand("DELETE FROM Disease WHERE DiseaseId=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
