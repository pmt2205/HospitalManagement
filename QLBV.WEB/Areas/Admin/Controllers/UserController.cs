using Microsoft.AspNetCore.Mvc;
using QLBV.DTO;
using QLBV.DAL.Entities;
using QLBV.DAL.Repositories;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace QLBV.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserRepository _userRepo;

        public UserController(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // GET: Admin/User
        public IActionResult Index()
        {
            var users = _userRepo.GetAll();
            return View(users);
        }

        // GET: Admin/User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/User/Create
        [HttpPost]
        public IActionResult Create(UserDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                ModelState.AddModelError("Password", "Password không được để trống");
                return View(dto);
            }

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password),
                FullName = dto.FullName,
                Email = dto.Email,
                Role = string.IsNullOrEmpty(dto.Role) ? "Patient" : dto.Role
            };

            _userRepo.Add(user);
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/User/Edit/5
        public IActionResult Edit(int id)
        {
            var user = _userRepo.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: Admin/User/Edit/5
        [HttpPost]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                _userRepo.Update(user);
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/User/Delete/5
        public IActionResult Delete(int id)
        {
            _userRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // Hash password an toàn
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }
    }
}
