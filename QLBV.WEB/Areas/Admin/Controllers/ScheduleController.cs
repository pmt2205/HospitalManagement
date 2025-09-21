using Microsoft.AspNetCore.Mvc;
using QLBV.DAL.Repositories;
using QLBV.DTO;
using Microsoft.AspNetCore.Authorization;

namespace QLBV.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ScheduleController : Controller
    {
        private readonly ScheduleRepository _scheduleRepo;
        private readonly DoctorRepository _doctorRepo;

        public ScheduleController(ScheduleRepository scheduleRepo, DoctorRepository doctorRepo)
        {
            _scheduleRepo = scheduleRepo;
            _doctorRepo = doctorRepo;
        }

        // GET: Admin/Schedule
        public IActionResult Index()
        {
            var schedules = _scheduleRepo.GetAll();
            return View(schedules);
        }

        // GET: Admin/Schedule/Details/5
        public IActionResult Details(int id)
        {
            var schedule = _scheduleRepo.GetScheduleById(id);
            if (schedule == null) return NotFound();
            return View(schedule);
        }

        // GET: Admin/Schedule/Create
        public IActionResult Create()
        {
            ViewBag.Doctors = _doctorRepo.GetAll();
            return View();
        }

        // POST: Admin/Schedule/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ScheduleDto dto)
        {
            if (ModelState.IsValid)
            {
                _scheduleRepo.Add(dto);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Doctors = _doctorRepo.GetAll();
            return View(dto);
        }

        // GET: Admin/Schedule/Edit/5
        public IActionResult Edit(int id)
        {
            var schedule = _scheduleRepo.GetScheduleById(id);
            if (schedule == null) return NotFound();
            ViewBag.Doctors = _doctorRepo.GetAll();
            return View(schedule);
        }

        // POST: Admin/Schedule/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ScheduleDto dto)
        {
            if (ModelState.IsValid)
            {
                _scheduleRepo.Update(dto);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Doctors = _doctorRepo.GetAll();
            return View(dto);
        }

        // GET: Admin/Schedule/Delete/5
        public IActionResult Delete(int id)
        {
            var schedule = _scheduleRepo.GetScheduleById(id);
            if (schedule == null) return NotFound();
            return View(schedule);
        }

        // POST: Admin/Schedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _scheduleRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
