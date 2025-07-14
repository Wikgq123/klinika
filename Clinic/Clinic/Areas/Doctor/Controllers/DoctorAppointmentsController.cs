
using Clinic.Data;
using Clinic.Enums;
using Clinic.Models;
using Clinic.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = SD.Role_Doctor)]
    public class DoctorAppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorAppointmentsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: Doctor/DoctorAppointments
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.ApplicationUserId == userId);
            if (doctor == null) return NotFound();

            var appointments = await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.LabExams)
                .Include(a => a.PhysicalExams)
                .Where(a => a.DoctorId == doctor.DoctorId)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }

        // GET: Doctor/DoctorAppointments/Archive
        public async Task<IActionResult> Archive()
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.ApplicationUserId == userId);
            if (doctor == null) return NotFound();

            var pastAppointments = await _db.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctor.DoctorId && a.AppointmentDate < System.DateTime.Now)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return View(pastAppointments);
        }

        // GET: Doctor/DoctorAppointments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.LabExams).ThenInclude(le => le.ExamSelection)
                .Include(a => a.PhysicalExams).ThenInclude(pe => pe.ExamSelection)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null) return NotFound();

            var previousVisits = await _db.Appointments
                .Where(a => a.PatientId == appointment.PatientId && a.AppointmentId != id)
                .OrderByDescending(a => a.AppointmentDate)
                .Take(5)
                .ToListAsync();

            var model = new DoctorAppointmentVM
            {
                Appointment = appointment,
                ExamSelections = await _db.ExamSelections.ToListAsync(),
                PreviousVisits = previousVisits,
                LabExamIds = appointment.LabExams?.Select(e => e.LabExamId).ToList(),
                LabExamStatuses = appointment.LabExams?.Select(e => e.Status).ToList()
            };

            return View(model);
        }

        // POST: Doctor/DoctorAppointments/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(DoctorAppointmentVM model)
        {
            // If validation failed, reload the full model
            if (!ModelState.IsValid)
            {
                var fullAppt = await _db.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.LabExams).ThenInclude(le => le.ExamSelection)
                    .Include(a => a.PhysicalExams).ThenInclude(pe => pe.ExamSelection)
                    .FirstOrDefaultAsync(a => a.AppointmentId == model.Appointment.AppointmentId);
                if (fullAppt == null) return NotFound();

                var previous = await _db.Appointments
                    .Where(a => a.PatientId == fullAppt.PatientId && a.AppointmentId != fullAppt.AppointmentId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .Take(5)
                    .ToListAsync();

                model.Appointment = fullAppt;
                model.ExamSelections = await _db.ExamSelections.ToListAsync();
                model.PreviousVisits = previous;
                model.LabExamIds = fullAppt.LabExams?.Select(e => e.LabExamId).ToList();
                model.LabExamStatuses = fullAppt.LabExams?.Select(e => e.Status).ToList();

                return View("Details", model);
            }

            var appointment = await _db.Appointments.FindAsync(model.Appointment.AppointmentId);
            if (appointment == null) return NotFound();

            appointment.Status = model.Appointment.Status;
            appointment.Diagnosis = model.Appointment.Diagnosis;
            _db.Entry(appointment).State = EntityState.Modified;

            if (model.LabExamIds != null && model.LabExamStatuses != null)
            {
                for (int i = 0; i < model.LabExamIds.Count && i < model.LabExamStatuses.Count; i++)
                {
                    var exam = await _db.LabExams.FindAsync(model.LabExamIds[i]);
                    if (exam != null)
                    {
                        exam.Status = model.LabExamStatuses[i];
                        _db.LabExams.Update(exam);
                    }
                }
            }



            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Doctor/DoctorAppointments/Cancel/5
        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            var appt = await _db.Appointments.FindAsync(id);
            if (appt == null) return NotFound();

            appt.Status = AppointmentStatus.Cancelled;
            _db.Appointments.Update(appt);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
