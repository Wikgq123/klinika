using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Clinic.Models;
using Clinic.Data;
using Clinic.Models.ExamRecords;
using System;
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

            var exam = new Clinic.Models.ExamRecords.PhysicalExam
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
    }
}
