using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QLBV.BLL;
using QLBV.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QLBV.WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly IConfiguration _config;

        public AccountController(UserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(UserDto dto, string password)
        {
            if (ModelState.IsValid)
            {
                var success = _userService.Register(dto, password);
                if (success)
                    return RedirectToAction("Login");

                ModelState.AddModelError("", "Tên đăng nhập đã tồn tại!");
            }
            return View(dto);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _userService.Login(username, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu");
                return View();
            }

            // --- Tạo JWT token ---
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // --- Lưu JWT vào cookie ---
            Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // bật HTTPS
                Expires = DateTimeOffset.Now.AddHours(2),
                SameSite = SameSiteMode.Strict
            });

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Login");
        }
    }
}
