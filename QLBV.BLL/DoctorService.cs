using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLBV.DAL.Repositories;
using QLBV.DTO;

namespace QLBV.BLL
{
    public class DoctorService
    {
        private readonly DoctorRepository _repository;

        public DoctorService(DoctorRepository repository)
        {
            _repository = repository;
        }

        // Lấy danh sách bác sĩ theo DepartmentId
        public List<DoctorDto> GetDoctorsByDepartment(int departmentId)
        {
            return _repository.GetByDepartment(departmentId);
        }

        // Lấy bác sĩ theo Id
        public DoctorDto GetDoctorById(int doctorId)
        {
            return _repository.GetById(doctorId);
        }

    }
}
