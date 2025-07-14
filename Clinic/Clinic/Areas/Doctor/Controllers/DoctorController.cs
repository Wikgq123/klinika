using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = Clinic.Models.SD.Role_Doctor)]
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
