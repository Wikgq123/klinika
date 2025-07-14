using Clinic.Data;
using Clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Areas.HeadLabTechnician.Controllers
{
    [Area("HeadLabTechnician")]
    [Authorize(Roles = SD.Role_HeadLabTechnician)]
    public class LabExamsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LabExamsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Approve()
        {
            var exams = await _db.LabExams
                .Include(e => e.LabTechnician)
                    .ThenInclude(lt => lt.ApplicationUser)
                .Include(e => e.ExamSelection)
                .Where(e => e.Status == "Completed")
                .ToListAsync();
            return View(exams);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveExam(int id)
        {
            var exam = await _db.LabExams.FindAsync(id);
            if (exam == null) return NotFound();

            exam.Status = "Approved";
            exam.ApprovedBy = User.Identity?.Name;
            exam.ApprovedDate = DateTime.Now;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Approve));
        }
    }
}
