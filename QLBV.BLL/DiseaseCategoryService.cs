using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLBV.DAL.Repositories;
using QLBV.DTO;

namespace QLBV.BLL
{
    public class DiseaseCategoryService
    {
        private readonly DiseaseCategoryRepository _repository;

        public DiseaseCategoryService(DiseaseCategoryRepository repository)
        {
            _repository = repository;
        }

        // Lấy tất cả loại bệnh
        public List<DiseaseCategoryDto> GetAllCategories()
        {
            return _repository.GetAll();
        }

        // Lấy loại bệnh theo Id
        public DiseaseCategoryDto GetCategoryById(int categoryId)
        {
            var categories = _repository.GetAll();
            return categories.Find(c => c.DiseaseCategoryId == categoryId);
        }

        public List<DiseaseCategoryDto> GetCategoriesByDepartment(int departmentId)
        {
            return _repository.GetByDepartmentId(departmentId);
        }
    }
}
