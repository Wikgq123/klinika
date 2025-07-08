using Clinic.Data;
using Clinic.Enums;
using Clinic.Models;
using Clinic.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Clinic.Areas.Receptionist.Controllers
{

    [Area("Receptionist")]
    [Authorize(Roles = SD.Role_Receptionist)]
    public class PatientsController : Controller
    {

        private readonly ApplicationDbContext db;

        public PatientsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(string searchString, int? pageIndex, int pageSize = 10)
        {
            ViewData["CurrentFilter"] = searchString;

            IQueryable<Patient> patientsQuery = db.Patients.Include(x => x.Address);

            if (!string.IsNullOrEmpty(searchString))
            {
                patientsQuery = patientsQuery.Where(p =>
                    p.Name.Contains(searchString) ||
                    p.Surname.Contains(searchString) ||
                    p.PESEL.Contains(searchString));
            }

            int pageNumber = pageIndex ?? 1;
            var paginatedPatients = await PaginatedList<Patient>.CreateAsync(patientsQuery, pageNumber, pageSize);

            return View(paginatedPatients);
        }

        public IActionResult Create()
        {
            var model = new PatientVM
            {
                Patient = new Patient(),
                Addresses = db.Addresses.ToList()
            };
            return View(model);
        }

        public IActionResult CreateAddress()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateAddress(Address model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors });
                foreach (var error in errors)
                {
                    Console.WriteLine($"Key: {error.Key}, Errors: {string.Join(", ", error.Errors)}");
                }
            }

            if (ModelState.IsValid)
            {
                db.Addresses.Add(model);
                db.SaveChanges();
                TempData["Success"] = "Address added successfully!";
                return RedirectToAction("Create");
            }

            return View(model);
            
        }

        [HttpPost]
        public IActionResult Create(PatientVM model)
        {
            ModelState.Remove("Patient.Address");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors });
                foreach (var error in errors)
                {
                    Console.WriteLine($"Key: {error.Key}, Errors: {string.Join(", ", error.Errors)}");
                }
            }

            // TODO: Find out why patient is never valid
            if (ModelState.IsValid)
            {

                db.Patients.Add(model.Patient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            model.Addresses = db.Addresses.ToList();
            return View(model);
        }


        public IActionResult DeletePatient(int patientId)
        {
            var patient = db.Patients.Find(patientId);
            if (patient != null)
            {
                db.Patients.Remove(patient);
                db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }


        public IActionResult UpdatePatient(int patientId)
        {
            var patient = db.Patients.Find(patientId);
            if (patient == null)
                return NotFound();
            var model = new PatientVM
            {
                Patient = patient,
                Addresses = db.Addresses.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdatePatient(PatientVM model)
        {

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Error");
            }
            if (ModelState.IsValid)
            {
                var newPatient = model.Patient;
                var patient = db.Patients.Find(newPatient.PatientId);
                if (patient != null)
                {
                    patient.Surname = newPatient.Surname;
                    patient.PESEL = newPatient.PESEL;
                    patient.Name = newPatient.Name;
                    patient.AddressId = newPatient.AddressId;
                    db.SaveChanges();
                }
                else 
                {
                    Console.WriteLine("Patient is null");
                }
                return RedirectToAction(nameof(Index));
            }
            model.Addresses = db.Addresses.ToList();
            return View(model);
        }
    }
}
