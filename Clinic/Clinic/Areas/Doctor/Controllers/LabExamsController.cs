// File: Areas/Doctor/Controllers/LabExamsController.cs
using Clinic.Data;
using Clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = SD.Role_Doctor)]
    public class LabExamsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LabExamsController(ApplicationDbContext db) => _db = db;

        // GET: Doctor/LabExams/Order
        [HttpGet]
        public async Task<IActionResult> Order(int appointmentId)
        {
            var appointment = await _db.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
            if (appointment == null) return NotFound();

            ViewBag.Appointment = appointment;
            ViewBag.Exams = await _db.ExamSelections
                .Where(e => e.Type == Clinic.Enums.ExamType.Lab)
                .ToListAsync();
            return View();
        }

        // POST: Doctor/LabExams/Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order(int appointmentId, string examSelectionId)
        {
            var appointment = await _db.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
            if (appointment == null) return NotFound();

            var head = await _db.HeadLabTechnicians.FirstOrDefaultAsync();
            if (head == null) return NotFound();

            var labExam = new LabExam
            {
                AppointmentId = appointmentId,
                ExamSelectionId = examSelectionId,
                Status = "Pending",
                RequestDate = System.DateTime.Now,
                HeadLabTechnicianId = head.HeadLabTechnicianId
            };

            _db.LabExams.Add(labExam);
            appointment.Status = Clinic.Enums.AppointmentStatus.InProgress;
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", "DoctorAppointments", new { id = appointmentId });
        }

        // GET: Doctor/LabExams/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var exam = await _db.LabExams
                .Include(e => e.ExamSelection)
                .Include(e => e.Appointment)
                    .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(e => e.LabExamId == id);
            if (exam == null) return NotFound();
            return View(exam);
        }
    }
}
