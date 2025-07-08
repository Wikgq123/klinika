using Clinic.Data;
using Clinic.Models;
using Clinic.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace Clinic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext db;
        private readonly RoleManager<IdentityRole> roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.db = db;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string userName)
        {
            var users = userManager.Users.Include(x => x.Doctor).ToList();
            if (!String.IsNullOrEmpty(userName))
            {
                users = users.Where(x => (x.Surname + x.Name).ToLower().Contains(userName.ToLower())).ToList();
            }

            var model = new List<UserWithRole>();
            foreach (var user in users)
            {
                var role = await userManager.GetRolesAsync(user);
                if (!role.IsNullOrEmpty())
                {
                    model.Add(new UserWithRole
                    {
                        User = user,
                        Role = role.First()
                    });
                }
                else
                {
                    model.Add(new UserWithRole
                    {
                        User = user,
                        Role = " - "
                    });
                }
            }
            return View(model);
        }
        public IActionResult CreateUser()
        {
            return RedirectToPage("/Account/Register", new { area = "Identity" });
        }

        public async Task<ActionResult> SetActive(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (user.IsActive)
                    user.IsActive = false;
                else 
                    user.IsActive = true;
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View();
            
        }

        public async Task<IActionResult> UpdateUser(string userId)
        {
            var user = db.Users.Include(x => x.Doctor).FirstOrDefault(x => x.Id == userId);
            if (user != null)
            {
                var model = new UpdateUserVM
                {
                    ApplicationUser = user,
                    Role = (await userManager.GetRolesAsync(user)).FirstOrDefault(),
                    RoleList = roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                    {
                        Text = i,
                        Value = i
                    })
                };
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateUser(UpdateUserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Include(x => x.Doctor).FirstOrDefault(x => x.Id == model.ApplicationUser.Id);
                if (user != null)
                {
                    var currentRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
                    var newRole = model.Role;

                        user.Name = model.ApplicationUser.Name;
                        user.Surname = model.ApplicationUser.Surname;
                        user.Email = model.ApplicationUser.Email;
                        user.UserName = model.ApplicationUser.UserName;


                    if (currentRole == newRole)
                    {
                        if (model.ApplicationUser.Doctor.DoctorId != null)
                        {
                            user.Doctor.NPWZ = model.ApplicationUser.Doctor.NPWZ;
                        }
                        await userManager.UpdateAsync(user);
                    }
                    else
                    {
                       await UpdateUserRoleAsync(user, model);
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            model.RoleList = roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
            {
                Text = i,
                Value = i
            });

            return View(model);

        }



        private async Task UpdateUserRoleAsync(ApplicationUser user, UpdateUserVM model)
        {

            var oldRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
            if (oldRole != null)
                await userManager.RemoveFromRoleAsync(user, oldRole);

            switch (oldRole)
            {
                case SD.Role_Doctor:
                    var doctor = db.Doctors.FirstOrDefault(d => d.ApplicationUserId == user.Id);
                    if (doctor != null) db.Doctors.Remove(doctor);
                    break;
                case SD.Role_Receptionist:
                    var receptionist = db.Receptionists.FirstOrDefault(r => r.ApplicationUserId == user.Id);
                    if (receptionist != null) db.Receptionists.Remove(receptionist);
                    break;
                case SD.Role_Admin:
                    var admin = db.Admins.FirstOrDefault(a => a.ApplicationUserId == user.Id);
                    if (admin != null) db.Admins.Remove(admin);
                    break;
                case SD.Role_LabTechnician:
                    var lab = db.LabTechnicians.FirstOrDefault(l => l.ApplicationUserId == user.Id);
                    if (lab != null) db.LabTechnicians.Remove(lab);
                    break;
                case SD.Role_HeadLabTechnician:
                    var head = db.HeadLabTechnicians.FirstOrDefault(h => h.ApplicationUserId == user.Id);
                    if (head != null) db.HeadLabTechnicians.Remove(head);
                    break;
            }

            var result = await userManager.AddToRoleAsync(user, model.Role);

            switch (model.Role)
            {
                case SD.Role_Receptionist:
                    db.Receptionists.Add(new Models.Receptionist
                    {
                        ApplicationUserId = user.Id,
                    });
                    break;
                case SD.Role_Doctor:
                    db.Doctors.Add(new Clinic.Models.Doctor
                    {
                        ApplicationUserId = user.Id,
                        NPWZ = model.ApplicationUser.Doctor?.NPWZ
                    });
                    break;
                case SD.Role_Admin:
                    db.Admins.Add(new Models.Admin
                    {
                        ApplicationUserId = user.Id
                    });
                    break;
                case SD.Role_LabTechnician:
                    db.LabTechnicians.Add(new Models.LabTechnician
                    {
                        ApplicationUserId = user.Id
                    });
                    break;
                case SD.Role_HeadLabTechnician:
                    db.HeadLabTechnicians.Add(new Models.HeadLabTechnician
                    {
                        ApplicationUserId = user.Id
                    });
                    break;
                default:
                    return;
            }
            db.SaveChanges();
        }

    }
}
