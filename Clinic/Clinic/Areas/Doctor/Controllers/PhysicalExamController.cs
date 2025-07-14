using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Clinic.Models;

namespace Clinic.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = SD.Role_Doctor)]
    public class PhysicalExamController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
