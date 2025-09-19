using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLBV.DAL.Repositories;
using QLBV.DTO;

namespace QLBV.BLL
{
    public class DiseaseService
    {
        private readonly DiseaseRepository _repository;

        public DiseaseService(DiseaseRepository repository)
        {
            _repository = repository;
        }

        // Lấy danh sách bệnh theo loại bệnh
        public List<DiseaseDto> GetDiseasesByCategory(int categoryId)
        {
            return _repository.GetByCategory(categoryId);
        }

        // Lấy bệnh theo Id
        public DiseaseDto GetDiseaseById(int diseaseId)
        {
            var diseases = _repository.GetByCategory(0); // tạm thời, hoặc viết thêm GetById trong repo
            return diseases.Find(d => d.DiseaseId == diseaseId);
        }
    }
}
