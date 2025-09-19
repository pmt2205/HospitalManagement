using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using QLBV.BLL;
using QLBV.DTO;
using QLBV.DAL.Repositories;

namespace QLBV.WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly PatientRepository _patientRepository;


        public AccountController(UserService userService, PatientRepository patientRepository)
        {
            _userService = userService;
            _patientRepository = patientRepository;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserDto dto, string password)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var success = _userService.Register(dto, password);
            if (!success)
            {
                ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                return View(dto);
            }

            TempData["Success"] = "Đăng ký thành công! Bạn có thể đăng nhập.";
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _userService.Login(username, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme
            );

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
