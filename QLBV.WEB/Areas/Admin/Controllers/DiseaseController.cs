using Microsoft.AspNetCore.Mvc;
using QLBV.DAL.Repositories;
using QLBV.DTO;
using Microsoft.AspNetCore.Authorization;

namespace QLBV.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiseaseController : Controller
    {
        private readonly DiseaseRepository _repo;
        private readonly DiseaseCategoryRepository _categoryRepo;

        public DiseaseController(DiseaseRepository repo, DiseaseCategoryRepository categoryRepo)
        {
            _repo = repo;
            _categoryRepo = categoryRepo;
        }

        // GET: Admin/Disease
        public IActionResult Index()
        {
            var list = _repo.GetAll();
            return View(list);
        }

        // GET: Admin/Disease/Details/5
        public IActionResult Details(int id)
        {
            var disease = _repo.GetById(id);
            if (disease == null) return NotFound();
            return View(disease);
        }

        // GET: Admin/Disease/Create
        public IActionResult Create()
        {
            ViewBag.Categories = _categoryRepo.GetAll();
            return View();
        }

        // POST: Admin/Disease/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DiseaseDto dto)
        {
            if (ModelState.IsValid)
            {
                _repo.Add(dto);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _categoryRepo.GetAll();
            return View(dto);
        }

        // GET: Admin/Disease/Edit/5
        public IActionResult Edit(int id)
        {
            var disease = _repo.GetById(id);
            if (disease == null) return NotFound();
            ViewBag.Categories = _categoryRepo.GetAll();
            return View(disease);
        }

        // POST: Admin/Disease/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DiseaseDto dto)
        {
            if (ModelState.IsValid)
            {
                _repo.Update(dto);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _categoryRepo.GetAll();
            return View(dto);
        }

        // GET: Admin/Disease/Delete/5
        public IActionResult Delete(int id)
        {
            var disease = _repo.GetById(id);
            if (disease == null) return NotFound();
            return View(disease);
        }

        // POST: Admin/Disease/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
