using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLBV.DAL.Entities;
using QLBV.DAL.Repositories;
using QLBV.DTO;
using System.Security.Cryptography;

namespace QLBV.BLL
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool Register(UserDto dto, string password)
        {
            var existing = _userRepository.GetByUsername(dto.Username);
            if (existing != null) return false;

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = HashPassword(password),
                FullName = dto.FullName,
                Email = dto.Email,
                Role = "Patient"
            };

            _userRepository.Add(user);
            return true;
        }

        public UserDto Login(string username, string password)
        {
            var user = _userRepository.GetByUsername(username);
            if (user != null && user.PasswordHash == HashPassword(password))
            {
                return new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role
                };
            }
            return null;
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
