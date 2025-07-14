using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Clinic.Models;
using Clinic.Data;
using Microsoft.EntityFrameworkCore;
using PhysicalExamMain = Clinic.Models.PhysicalExam;
using PhysicalExamRecord = Clinic.Models.ExamRecords.PhysicalExam;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = SD.Role_Doctor)]
    public class PhysicalExamController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public PhysicalExamController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var exams = new[] { "Heart Check", "Lung Auscultation", "Abdomen Palpation" };
            return View(exams);
        }

        [HttpGet]
        public IActionResult Start(string examType)
        {
            if (string.IsNullOrEmpty(examType)) return RedirectToAction(nameof(Index));
            ViewBag.ExamType = examType;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(string examType, int patientId, string notes)
        {
            if (string.IsNullOrEmpty(examType)) return RedirectToAction(nameof(Index));

            var exam = new PhysicalExamRecord
            {
                ExamType = examType,
                Notes = notes,
                PatientId = patientId,
                DoctorId = _userManager.GetUserId(User),
                Date = DateTime.Now
            };

            _db.PhysicalExamRecords.Add(exam);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Doctor/PhysicalExam/Order
        [HttpGet]
        public async Task<IActionResult> Order(int appointmentId)
        {
            var appointment = await _db.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
            if (appointment == null) return NotFound();

            ViewBag.Appointment = appointment;
            ViewBag.Exams = await _db.ExamSelections
                .Where(e => e.Type == Clinic.Enums.ExamType.Physical)
                .ToListAsync();
            return View();
        }

        // POST: Doctor/PhysicalExam/Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order(int appointmentId, string examSelectionId, string notes)
        {
            var appointment = await _db.Appointments.FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
            if (appointment == null) return NotFound();

            var exam = new PhysicalExamMain
            {
                AppointmentId = appointmentId,
                ExamSelectionId = examSelectionId,
                Result = notes
            };

            _db.PhysicalExams.Add(exam);
            appointment.Status = Clinic.Enums.AppointmentStatus.InProgress;
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", "DoctorAppointments", new { id = appointmentId });
        }
    }
}
