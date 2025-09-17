using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QLBV.WEB.Models;

namespace QLBV.WEB.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
