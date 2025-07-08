using Clinic.Data;
using Clinic.Enums;
using Clinic.Models;
using Clinic.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Areas.Receptionist.Controllers
{
    [Area("Receptionist")]
    [Authorize(Roles = SD.Role_Receptionist)]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;

        public AppointmentsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index(
    string patientName,
    string doctorName,
    int? pageIndex,
    int pageSize = 10)
        {
            ViewData["PatientNameFilter"] = patientName;
            ViewData["DoctorNameFilter"] = doctorName;

            IQueryable<Appointment> appointmentsQuery = db.Appointments
                .Include(x => x.Doctor)
                    .ThenInclude(x => x.ApplicationUser)
                .Include(x => x.Patient)
                .AsNoTracking();

            // Filtrowanie po pacjencie
            if (!string.IsNullOrEmpty(patientName))
            {
                appointmentsQuery = appointmentsQuery.Where(x =>
                    (x.Patient.Name + " " + x.Patient.Surname)
                        .ToLower()
                        .Contains(patientName.Trim().ToLower()));
            }

            // Filtrowanie po lekarzu
            if (!string.IsNullOrEmpty(doctorName))
            {
                appointmentsQuery = appointmentsQuery.Where(x =>
                    (x.Doctor.ApplicationUser.Name + " " + x.Doctor.ApplicationUser.Surname)
                        .ToLower()
                        .Contains(doctorName.Trim().ToLower()));
            }

            // Stronicowanie
            int pageNumber = pageIndex ?? 1;
            var paginatedAppointments = await PaginatedList<Appointment>
                .CreateAsync(appointmentsQuery, pageNumber, pageSize);

            return View(paginatedAppointments);
        }

        public IActionResult CreateAppointment()
        {
            var model = new AppointmentVM
            {
                Appointment = new Appointment(),
                Patients = db.Patients.ToList(),
                Doctors = db.Doctors.Include(x => x.ApplicationUser).ToList(),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateAppointment(AppointmentVM model)
        {
            if (ModelState.IsValid)
            {
                model.Appointment.Status = AppointmentStatus.Awaiting;
                model.Appointment.RegistrationDate = DateTime.Now;

                var userId = userManager.GetUserId(User);
                var recpetionist = db.Receptionists.FirstOrDefault(x => x.ApplicationUserId == userId);
                if (recpetionist != null)
                    model.Appointment.ReceptionistId = recpetionist.ReceptionistId;
                else
                    return NotFound();

                var doctor = db.Doctors.FirstOrDefault(x => x.DoctorId == model.Appointment.DoctorId);
                var appointmentTime = model.Appointment.AppointmentDate;

                var hasConflict = findConflict(model);
                if (hasConflict)
                {
                    ModelState.AddModelError("Appointment.AppointmentDate", "The selected doctor already has an appointment within 30 minutes of this time.");
                    model.Patients = db.Patients.ToList();
                    model.Doctors = db.Doctors.Include(x => x.ApplicationUser).ToList();
                    return View(model);
                }

                db.Appointments.Add(model.Appointment);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            model.Patients = db.Patients.ToList();
            model.Doctors = db.Doctors.Include(x => x.ApplicationUser).ToList();
            return View(model);
        }

        public IActionResult UpdateAppointment(int appointmentId)
        {
            var appointment = db.Appointments.Find(appointmentId);
            if (appointment == null)
                return NotFound();
            var model = new AppointmentVM
            {
                Appointment = appointment,
                Doctors = db.Doctors.Include(x => x.ApplicationUser).ToList(),
                Patients = db.Patients.ToList(),
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateAppointment(AppointmentVM model)
        {
            if (ModelState.IsValid)
            {
                var newAppointment = model.Appointment;
                var appointment = db.Appointments.Find(newAppointment.AppointmentId);
                var hasConflict = findConflict(model);
                if (hasConflict)
                {
                    ModelState.AddModelError("Appointment.AppointmentDate", "The selected doctor already has an appointment within 30 minutes of this time.");
                    model.Patients = db.Patients.ToList();
                    model.Doctors = db.Doctors.Include(x => x.ApplicationUser).ToList();
                    return View(model);
                }
                if (appointment != null)
                {
                    appointment.AppointmentDate = newAppointment.AppointmentDate;
                    appointment.DoctorId = newAppointment.DoctorId;
                    appointment.Description = newAppointment.Description;
                    appointment.PatientId = newAppointment.PatientId;
                    db.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            model.Patients = db.Patients.ToList();
            model.Doctors = db.Doctors.Include(x => x.ApplicationUser).ToList();
            return View(model);
        }

        public IActionResult DeleteAppointment(int appointmentId)
        {
            var appointment = db.Appointments.Find(appointmentId);
            if (appointment != null)
            {
                db.Appointments.Remove(appointment);
                db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool findConflict(AppointmentVM model)
        {
            var doctor = db.Doctors.FirstOrDefault(x => x.DoctorId == model.Appointment.DoctorId);
            var appointmentTime = model.Appointment.AppointmentDate;

            var hasConflict = db.Appointments.Any(x => x.DoctorId == doctor.DoctorId && x.AppointmentId != model.Appointment.AppointmentId && x.AppointmentDate >= appointmentTime.Value.AddMinutes(-30) && x.AppointmentDate <= appointmentTime.Value.AddMinutes(30));

            return hasConflict;

        }
    }
}
