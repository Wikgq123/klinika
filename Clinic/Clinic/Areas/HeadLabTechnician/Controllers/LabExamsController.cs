using Clinic.Data;
using Clinic.Enums;
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

        public async Task<IActionResult> Index()
        {
            var exams = await _db.LabExams
                .Include(e => e.LabTechnician)
                    .ThenInclude(lt => lt.ApplicationUser)
                .Include(e => e.ExamSelection)
                .ToListAsync();
            return View(exams);
        }

        [HttpPost]
        public IActionResult AcceptExam(int id, string headNotes)
        {
            var exam = _db.LabExams.FirstOrDefault(e => e.LabExamId == id);
            if (exam != null)
            {
                exam.HeadLabNotes = headNotes;
                exam.AcceptDate = DateTime.Now;
                exam.Status = ExamStatus.Completed;
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
