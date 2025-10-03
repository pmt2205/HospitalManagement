using Microsoft.AspNetCore.Mvc;
using QLBV.DAL.Repositories;
using QLBV.DTO;
using Microsoft.AspNetCore.Authorization;

namespace QLBV.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly DepartmentRepository _deptRepo;

        public DepartmentController(DepartmentRepository deptRepo)
        {
            _deptRepo = deptRepo;
        }

        // GET: Admin/Department
        public IActionResult Index()
        {
            var list = _deptRepo.GetAll();
            return View(list);
        }

        // GET: Admin/Department/Details/5
        public IActionResult Details(int id)
        {
            var dept = _deptRepo.GetById(id);
            if (dept == null) return NotFound();
            return View(dept);
        }

        // GET: Admin/Department/Create
        public IActionResult Create()
        {
            var model = new DepartmentDto(); // khởi tạo instance mới
            return View(model);
        }


        // POST: Admin/Department/Create
        [HttpPost]
        [Area("Admin")]
        public IActionResult Create(DepartmentDto dept)
        {
            _deptRepo.Add(dept); // dữ liệu chắc chắn bind được
            return RedirectToAction("Index");
        }




        // GET: Admin/Department/Edit/5
        public IActionResult Edit(int id)
        {
            var dept = _deptRepo.GetById(id);
            if (dept == null) return NotFound();
            return View(dept);
        }

        // POST: Admin/Department/Edit/5
        [HttpPost]
        public IActionResult Edit(DepartmentDto dept)
        {
            if (ModelState.IsValid)
            {
                _deptRepo.Update(dept);
                return RedirectToAction(nameof(Index));
            }
            return View(dept);
        }

        // GET: Admin/Department/Delete/5
        public IActionResult Delete(int id)
        {
            var dept = _deptRepo.GetById(id);
            if (dept == null) return NotFound();
            return View(dept);
        }

        // POST: Admin/Department/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _deptRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
