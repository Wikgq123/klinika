using Clinic.Data;
using Clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Clinic.Areas.LabTechnician.Controllers
{
    [Area("LabTechnician")]
    [Authorize(Roles = SD.Role_LabTechnician)]
    public class LabExamsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LabExamsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var exams = await _db.LabExams
                .Include(e => e.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(e => e.ExamSelection)
                .Where(e => e.Status == "Pending")
                .ToListAsync();
            return View(exams);
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int id, string result)
        {
            var exam = await _db.LabExams.FindAsync(id);
            if (exam == null) return NotFound();

            exam.Result = result;
            exam.Status = "Completed";
            exam.PerformedBy = User.Identity?.Name;
            exam.ExamDate = DateTime.Now;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
