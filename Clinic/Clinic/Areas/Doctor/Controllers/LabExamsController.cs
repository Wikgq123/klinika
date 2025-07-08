// File: Areas/Doctor/Controllers/LabExamsController.cs
using Clinic.Data;
using Clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Clinic.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = SD.Role_Doctor)]
    public class LabExamsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LabExamsController(ApplicationDbContext db) => _db = db;

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
