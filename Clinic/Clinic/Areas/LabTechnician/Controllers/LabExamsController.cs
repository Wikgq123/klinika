using Clinic.Data;
using Clinic.Enums;
using Clinic.Models;
using Clinic.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;
using System.Security.Claims;


namespace Clinic.Areas.LabTechnician.Controllers
{
    [Area("LabTechnician")]
    [Authorize(Roles = SD.Role_LabTechnician)]
    public class LabExamsController : Controller
    {
        private readonly ApplicationDbContext db;

        public LabExamsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(string searchString, int? pageIndex, int pageSize = 10)
        {
            ViewData["CurrentFilter"] = searchString;

            IQueryable<LabExam> labExamsQuery = db.LabExams
                .Include(x => x.LabTechnician)
                .ThenInclude(x => x.ApplicationUser);

            // Jeśli istnieje funkcjonalność wyszukiwania
            if (!string.IsNullOrEmpty(searchString))
            {
                labExamsQuery = labExamsQuery.Where(x =>
                    x.LabTechnician.ApplicationUser.Name.Contains(searchString) ||
                    x.LabTechnician.ApplicationUser.Surname.Contains(searchString));
            }


            int pageNumber = pageIndex ?? 1;
            var paginatedExams = await PaginatedList<LabExam>.CreateAsync(labExamsQuery, pageNumber, pageSize);

            return View(paginatedExams);
        }


        public IActionResult LabExam(int labExamId)
        {

            var labExam = db.LabExams
                .Include(x => x.ExamSelection)
                .Include(x => x.Appointment)
                .FirstOrDefault(x => x.LabExamId == labExamId);

            var model = new LabExamVM
            {
                LabExam = labExam,
                ExamSelection = labExam?.ExamSelection
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult LabExam(LabExamVM model)
        {

            ModelState.Remove("LabExam.Appointment");
            ModelState.Remove("LabExam.LabTechnician");
            ModelState.Remove("LabExam.HeadLabTechnician");
            ModelState.Remove("LabExam.ExamSelection");

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
                var newLabExam = model.LabExam;
                var labExam = db.LabExams.Find(newLabExam.LabExamId);
                if (labExam != null)
                {

                    labExam.DoctorsNotes = newLabExam.DoctorsNotes;
                    labExam.RequestDate = newLabExam.RequestDate;
                    labExam.Result = newLabExam.Result;
                    labExam.ExamDate = newLabExam.ExamDate;
                    labExam.HeadLabNotes = newLabExam.HeadLabNotes;
                    labExam.AcceptDate = newLabExam.AcceptDate;
                    labExam.Status = ExamStatus.InProgress;

                    labExam.AppointmentId = newLabExam.AppointmentId;
                    labExam.LabTechnicianId = newLabExam.LabTechnicianId;
                    labExam.HeadLabTechnicianId = newLabExam.HeadLabTechnicianId;
                    labExam.ExamSelectionId = newLabExam.ExamSelectionId;


                    string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var labTechnician = db.LabTechnicians.FirstOrDefault(lt => lt.ApplicationUserId == userId);
                    if (labTechnician != null)
                    {
                        labExam.LabTechnicianId = labTechnician.LabTechnicianId;
                        labExam.ExamDate = DateTime.Now;
                    }
                    else
                    {
                        labExam.LabTechnicianId = null;
                        Console.WriteLine("error assigning labtechnician, labtechnician ID is null");
                    }



                    db.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Lab exam is null");
                }
                return RedirectToAction(nameof(Index));
            }
            


                return View(model);
        }

        [HttpPost]
        public IActionResult CancelExam(int labExamId, string cancelReason)
        {
            var labExam = db.LabExams.FirstOrDefault(x => x.LabExamId == labExamId);
            if (labExam != null)
            {
                labExam.Status = ExamStatus.Canceled;
                labExam.CancelationReason = cancelReason;
                Console.WriteLine($"Canceling lab exam {labExamId} with reason: {cancelReason}");
                db.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
