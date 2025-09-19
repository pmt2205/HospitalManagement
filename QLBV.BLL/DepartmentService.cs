using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLBV.DAL.Repositories;
using QLBV.DTO;

namespace QLBV.BLL
{
    public class DepartmentService
    {
        private readonly DepartmentRepository _repository;

        public DepartmentService(DepartmentRepository repository)
        {
            _repository = repository;
        }

        // Lấy tất cả khoa
        public List<DepartmentDto> GetAllDepartments()
        {
            return _repository.GetAll();
        }

        // Lấy khoa theo Id
        public DepartmentDto GetDepartmentById(int id)
        {
            return _repository.GetById(id);
        }
    }
}
