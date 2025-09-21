using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLBV.DTO;

namespace QLBV.DAL.Repositories
{
    public class DiseaseRepository
    {
        private readonly string _conn;
        public DiseaseRepository(string connectionString) => _conn = connectionString;

        public List<DiseaseDto> GetByCategory(int categoryId)
        {
            var list = new List<DiseaseDto>();
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT DiseaseId, Name FROM Disease WHERE CategoryId=@CategoryId", conn);
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DiseaseDto
                        {
                            DiseaseId = (int)reader["DiseaseId"],
                            Name = reader["Name"].ToString(),
                            CategoryId = categoryId
                        });
                    }
                }
            }
            return list;
        }

        public DiseaseDto GetById(int diseaseId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT DiseaseId, Name, Description
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
                    }
                }
            }
            return null;
        }
    }
}
