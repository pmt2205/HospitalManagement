using Microsoft.AspNetCore.Mvc;
using QLBV.BLL;
using System.Security.Cryptography;
using System.Text;

namespace QLBV.WEB.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IConfiguration _config;
        private readonly AppointmentService _appointmentService;

        public PaymentController(IConfiguration config, AppointmentService appointmentService)
        {
            _config = config;
            _appointmentService = appointmentService;
        }

        public IActionResult Result()
        {
            var qs = Request.Query;

            string partnerCode = qs["partnerCode"];
            string orderId = qs["orderId"];
            string requestId = qs["requestId"];
            string amount = qs["amount"];
            string orderInfo = qs["orderInfo"];
            string orderType = qs["orderType"];
            string transId = qs["transId"];
            string resultCode = qs["resultCode"];
            string message = qs["message"];
            string payType = qs["payType"];
            string responseTime = qs["responseTime"];
            string extraData = qs["extraData"];
            string signature = qs["signature"];

            string secretKey = _config["Momo:SecretKey"];

            // --- Build rawHash chuẩn MoMo ---
            string rawHash =
                $"partnerCode={partnerCode}&orderId={orderId}&requestId={requestId}&amount={amount}&orderType={orderType}&transId={transId}&resultCode={resultCode}&message={message}&payType={payType}&responseTime={responseTime}&extraData={extraData}";

            // --- HMAC SHA256 ---
            string computedSignature;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
                computedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }

            bool isValid = string.Equals(signature, computedSignature, StringComparison.OrdinalIgnoreCase);
            ViewBag.IsValid = isValid;

            // --- Debug ---
            ViewBag.DebugSignature = signature;
            ViewBag.DebugComputed = computedSignature;
            ViewBag.RawHash = rawHash;

            // --- Fetch chi tiết appointment ---
            int appointmentId = int.Parse(orderId.Split('_')[0]);
            var appointment = _appointmentService.GetAppointmentById(appointmentId);
            ViewBag.Appointment = appointment;

            if (appointment != null)
            {
                var patient = _appointmentService.GetPatientById(appointment.PatientId);
                ViewBag.Patient = patient;

                var doctor = _appointmentService.GetDoctorById(appointment.DoctorId);
                ViewBag.Doctor = doctor;

                var department = _appointmentService.GetDepartmentById(doctor.DepartmentId);
                ViewBag.Department = department;

                // Lấy tên bệnh
                var disease = _appointmentService.GetDiseaseById(appointment.DiseaseId);
                ViewBag.DiseaseName = disease?.Name ?? "Không xác định";
            }


            // --- Cập nhật trạng thái nếu hợp lệ ---
            if (isValid && resultCode == "0")
            {
                try
                {
                    _appointmentService.MarkPaid(appointmentId, "Momo");
                    ViewBag.Success = true;
                    ViewBag.Message = "Thanh toán thành công và lịch khám đã được xác nhận!";
                }
                catch (Exception ex)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = "Thanh toán thành công nhưng không cập nhật được lịch khám: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = message ?? "Thanh toán thất bại hoặc chữ ký không hợp lệ.";
            }

            // --- Thông tin hiển thị ---
            ViewBag.OrderId = orderId;
            ViewBag.Amount = amount;
            ViewBag.TransId = transId;
            ViewBag.OrderInfo = System.Net.WebUtility.UrlDecode(orderInfo ?? "");
            ViewBag.PayType = payType;

            return View();
        }
    }
}
