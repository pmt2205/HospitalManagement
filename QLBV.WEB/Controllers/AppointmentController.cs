using Microsoft.AspNetCore.Mvc;
using QLBV.BLL;
using QLBV.DTO;
using System.Security.Claims;

namespace QLBV.WEB.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly DepartmentService _departmentService;
        private readonly DoctorService _doctorService;
        private readonly DiseaseCategoryService _diseaseCategoryService;
        private readonly DiseaseService _diseaseService;
        private readonly MomoService _momoService;

        public AppointmentController(
            AppointmentService appointmentService,
            DepartmentService departmentService,
            DoctorService doctorService,
            DiseaseCategoryService diseaseCategoryService,
            DiseaseService diseaseService,
            MomoService momoService)
        {
            _appointmentService = appointmentService;
            _departmentService = departmentService;
            _doctorService = doctorService;
            _diseaseCategoryService = diseaseCategoryService;
            _diseaseService = diseaseService;
            _momoService = momoService;
        }

        // Bước 1: Chọn khoa
        public IActionResult Index()
        {
            var departments = _departmentService.GetAllDepartments();
            return View(departments);
        }

        // Bước 2: Hiển thị bác sĩ theo khoa
        public IActionResult Doctors(int departmentId)
        {
            var doctors = _doctorService.GetDoctorsByDepartment(departmentId);
            ViewBag.DepartmentId = departmentId; // giữ lại để quay về
            return View(doctors);
        }

        // Bước 3: Hiển thị lịch trống + chọn bệnh
        public IActionResult Schedule(int doctorId)
        {
            var doctor = _doctorService.GetDoctorById(doctorId);
            if (doctor == null)
            {
                TempData["Error"] = "Không tìm thấy bác sĩ.";
                return RedirectToAction("Index");
            }

            var schedules = _appointmentService.GetSchedulesByDoctor(doctorId);
            var diseaseCategories = _diseaseCategoryService.GetCategoriesByDepartment(doctor.DepartmentId);

            var department = _departmentService.GetDepartmentById(doctor.DepartmentId);
            var totalFee = department.BaseFee + doctor.ExtraFee;

            ViewBag.DoctorId = doctorId;
            ViewBag.DepartmentId = doctor.DepartmentId;
            ViewBag.DiseaseCategories = diseaseCategories;
            ViewBag.TotalFee = totalFee; // 👈 thêm dòng này

            return View(schedules);
        }


        // Bước 4: Lấy bệnh theo category (AJAX)
        public JsonResult Diseases(int categoryId)
        {
            var diseases = _diseaseService.GetDiseasesByCategory(categoryId);
            return Json(diseases);
        }

        // Bước 5: Đặt lịch + thanh toán
        [HttpPost]
        public async Task<IActionResult> Book(AppointmentDto dto, string paymentMethod)
        {
            // 1. Lấy PatientId từ claim
            var patientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (patientIdClaim == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập.";
                return RedirectToAction("Login", "Account");
            }
            dto.PatientId = int.Parse(patientIdClaim.Value);

            // 2. Lấy schedule
            var schedule = _appointmentService.GetScheduleById(dto.ScheduleId);
            if (schedule == null)
            {
                return ReturnToSchedule(dto.DoctorId, "Lịch khám không tồn tại.");
            }

            // 3. Gán AppointmentDate và Status
            dto.AppointmentDate = schedule.WorkDate.Date + schedule.StartTime;
            dto.Status = "Pending";

            // 4. Kiểm tra trùng lịch
            if (!_appointmentService.CheckScheduleAvailable(dto.ScheduleId))
            {
                return ReturnToSchedule(dto.DoctorId, "Thời gian này đã có người đặt.");
            }

            // 5. Tạo appointment và invoice
            int appointmentId;
            decimal amount;
            DoctorDto doctor;
            DepartmentDto department;
            try
            {
                appointmentId = _appointmentService.BookAppointment(dto, paymentMethod);

                doctor = _doctorService.GetDoctorById(dto.DoctorId);
                department = _departmentService.GetDepartmentById(doctor.DepartmentId);
                amount = department.BaseFee + doctor.ExtraFee;
            }
            catch (Exception ex)
            {
                return ReturnToSchedule(dto.DoctorId, "Không thể tạo lịch khám: " + ex.Message);
            }

            // 6. Thanh toán Momo nếu chọn
            if (paymentMethod == "Momo")
            {
                string orderInfo = $"Thanh toán lịch khám #{appointmentId}";

                // ✅ tạo orderId duy nhất
                string orderId = $"{appointmentId}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

                string payUrl = await _momoService.CreatePaymentUrlAsync(orderId, amount, orderInfo);

                if (string.IsNullOrEmpty(payUrl))
                {
                    return ReturnToSchedule(dto.DoctorId, "Không thể tạo link thanh toán Momo.");
                }

                return Redirect(payUrl);
            }


            // 7. Thành công
            return ReturnToSchedule(dto.DoctorId, null, "Đặt lịch thành công!");
        }

        /// <summary>
        /// Helper để return lại trang Schedule và luôn set đủ ViewBag
        /// </summary>
        private IActionResult ReturnToSchedule(int doctorId, string error = null, string success = null)
        {
            var doctor = _doctorService.GetDoctorById(doctorId);
            if (doctor == null) return RedirectToAction("Index");

            if (!string.IsNullOrEmpty(error)) TempData["Error"] = error;
            if (!string.IsNullOrEmpty(success)) TempData["Success"] = success;

            var schedules = _appointmentService.GetSchedulesByDoctor(doctorId);
            var diseaseCategories = _diseaseCategoryService.GetCategoriesByDepartment(doctor.DepartmentId);

            ViewBag.DoctorId = doctorId;
            ViewBag.DepartmentId = doctor.DepartmentId;
            ViewBag.DiseaseCategories = diseaseCategories;

            return View("Schedule", schedules);
        }
    }
}
