using System;
using System.Security.Cryptography;
using System.Text;
using QLBV.DAL.Entities;
using QLBV.DAL.Repositories;
using QLBV.DTO;

namespace QLBV.BLL
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly PatientRepository _patientRepository;

        public UserService(UserRepository userRepository, PatientRepository patientRepository)
        {
            _userRepository = userRepository;
            _patientRepository = patientRepository;
        }

        public bool Register(UserDto dto, string password)
        {
            // 1. Kiểm tra username
            var existing = _userRepository.GetByUsername(dto.Username);
            if (existing != null) return false;

            // 2. Tạo User
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = HashPassword(password),
                FullName = dto.FullName,
                Email = dto.Email,
                Role = string.IsNullOrEmpty(dto.Role) ? "Patient" : dto.Role
            };

            // Lấy UserId mới
            user.UserId = _userRepository.Add(user);

            // 3. Nếu Patient, tạo Patient record
            if (user.Role == "Patient")
            {
                var patient = new Patient
                {
                    UserId = user.UserId,
                    DateOfBirth = dto.DateOfBirth ?? DateTime.Now,
                    Gender = dto.Gender ?? "",
                    Address = dto.Address ?? ""
                };

                _patientRepository.Add(patient);
            }

            return true;
        }

        public UserDto Login(string username, string password)
        {
            var user = _userRepository.GetByUsername(username);
            if (user != null && user.PasswordHash == HashPassword(password))
            {
                Patient patient = null;
                if (user.Role == "Patient")
                    patient = _patientRepository.GetByUserId(user.UserId);

                return new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    DateOfBirth = patient?.DateOfBirth,
                    Gender = patient?.Gender,
                    Address = patient?.Address
                };
            }
            return null;
        }

        public UserDto GetByUsername(string username)
        {
            var user = _userRepository.GetByUsername(username);
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
