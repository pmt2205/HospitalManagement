using Microsoft.AspNetCore.Mvc;

namespace QLBV.WEB.Areas.Admin.Controllers
{
    [Area("Admin")] // Bắt buộc khai báo Area
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
