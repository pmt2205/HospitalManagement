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

        public List<DiseaseCategoryDto> GetAll()
        {
            var list = new List<DiseaseCategoryDto>();

            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var query = @"SELECT DiseaseCategoryId, Name, Description, DepartmentId 
                              FROM DiseaseCategory";
                var cmd = new SqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DiseaseCategoryDto
                        {
                            DiseaseCategoryId = Convert.ToInt32(reader["DiseaseCategoryId"]),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            DepartmentId = Convert.ToInt32(reader["DepartmentId"])
                        });
                    }
                }
            }

            return list;
        }

        public List<DiseaseCategoryDto> GetByDepartmentId(int departmentId)
        {
            var list = new List<DiseaseCategoryDto>();

            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var query = @"SELECT DiseaseCategoryId, Name, Description, DepartmentId 
                              FROM DiseaseCategory
                              WHERE DepartmentId = @depId";
                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@depId", departmentId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DiseaseCategoryDto
                        {
                            DiseaseCategoryId = Convert.ToInt32(reader["DiseaseCategoryId"]),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            DepartmentId = Convert.ToInt32(reader["DepartmentId"])
                        });
                    }
                }
            }

            return list;
        }
    }
}
