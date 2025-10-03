using QLBV.DAL.Repositories;
using QLBV.DAL.Entities;
using QLBV.DTO;

namespace QLBV.BLL
{
    public class PatientService
    {
        private readonly PatientRepository _patientRepo;

        public PatientService(PatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
        }

        public Patient GetByUserId(int userId)
        {
            return _patientRepo.GetByUserId(userId);
        }

        public PatientDto GetById(int patientId)
        {
            return _patientRepo.GetById(patientId);
        }

        public void Add(Patient patient)
        {
            _patientRepo.Add(patient);
        }
    }
}
