using Microsoft.AspNetCore.Mvc;
using QLBV.DAL.Repositories;
using QLBV.DTO;
using Microsoft.AspNetCore.Authorization;

namespace QLBV.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiseaseCategoryController : Controller
    {
        private readonly DiseaseCategoryRepository _repo;
        private readonly DepartmentRepository _deptRepo;

        public DiseaseCategoryController(DiseaseCategoryRepository repo, DepartmentRepository deptRepo)
        {
            _repo = repo;
            _deptRepo = deptRepo;
        }

        // GET: Admin/DiseaseCategory
        public IActionResult Index()
        {
            var list = _repo.GetAll();
            return View(list);
        }

        // GET: Admin/DiseaseCategory/Details/5
        public IActionResult Details(int id)
        {
            var category = _repo.GetById(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // GET: Admin/DiseaseCategory/Create
        // GET: Admin/DiseaseCategory/Create
        public IActionResult Create()
        {
            // Lấy danh sách khoa
            var depts = _deptRepo.GetAll();

            // Kiểm tra debug xem có bao nhiêu phần tử
            System.Diagnostics.Debug.WriteLine($"Department count: {depts?.Count ?? 0}");

            // Gán vào ViewBag để view dùng
            ViewBag.Departments = depts ?? new List<DepartmentDto>();

            return View(new DiseaseCategoryDto());
        }


        // POST: Create
        [HttpPost]
        public IActionResult Create(DiseaseCategoryDto dto)
        {
            if (ModelState.IsValid)
            {
                _repo.Add(dto);
                return RedirectToAction(nameof(Index));
            }

            // Nếu validate lỗi, bind lại dropdown
            ViewBag.Departments = _deptRepo.GetAll() ?? new List<DepartmentDto>();
            return View(dto);
        }


        // GET: Admin/DiseaseCategory/Edit/5
        public IActionResult Edit(int id)
        {
            var category = _repo.GetById(id);
            if (category == null) return NotFound();
            ViewBag.Departments = _deptRepo.GetAll();
            return View(category);
        }

        // POST: Admin/DiseaseCategory/Edit/5
        [HttpPost]
        public IActionResult Edit(DiseaseCategoryDto dto)
        {
            if (ModelState.IsValid)
            {
                _repo.Update(dto);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = _deptRepo.GetAll();
            return View(dto);
        }

        // GET: Admin/DiseaseCategory/Delete/5
        public IActionResult Delete(int id)
        {
            var category = _repo.GetById(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: Admin/DiseaseCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
