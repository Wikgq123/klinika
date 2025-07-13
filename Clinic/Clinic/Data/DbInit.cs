using Clinic.Models;
using Microsoft.AspNetCore.Identity;

namespace Clinic.Data
{
    public class DbInit
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = {
                SD.Role_Admin,
                SD.Role_Doctor,
                SD.Role_Receptionist,
                SD.Role_LabTechnician,
                SD.Role_HeadLabTechnician
            };

            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            await CreateUser(userManager, dbContext, "admin@admin.com", "AdminAdmin1!", SD.Role_Admin,
                user => dbContext.Admins.Add(new Admin { ApplicationUserId = user.Id }));

            await CreateUser(userManager, dbContext, "doctor@clinic.com", "Doctor123!", SD.Role_Doctor,
                user => dbContext.Doctors.Add(new Doctor { ApplicationUserId = user.Id }));

            await CreateUser(userManager, dbContext, "reception@clinic.com", "Reception123!", SD.Role_Receptionist,
                user => dbContext.Receptionists.Add(new Receptionist { ApplicationUserId = user.Id }));

            await CreateUser(userManager, dbContext, "lab@clinic.com", "Labtech123!", SD.Role_LabTechnician,
                user => dbContext.LabTechnicians.Add(new LabTechnician { ApplicationUserId = user.Id }));

            await CreateUser(userManager, dbContext, "headlab@clinic.com", "Headlab123!", SD.Role_HeadLabTechnician,
                user => dbContext.HeadLabTechnicians.Add(new HeadLabTechnician { ApplicationUserId = user.Id }));

            dbContext.SaveChanges();
        }

        private static async Task CreateUser(UserManager<ApplicationUser> userManager, ApplicationDbContext context,
            string email, string password, string role, Action<ApplicationUser> createRelatedEntity)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    Name = email.Split('@')[0],
                    Surname = "User"
                };
                await userManager.CreateAsync(user, password);
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }

            if (createRelatedEntity != null && !RelatedExists(context, role, user.Id))
            {
                createRelatedEntity(user);
            }
        }

        private static bool RelatedExists(ApplicationDbContext context, string role, string userId)
        {
            return role switch
            {
                SD.Role_Admin => context.Admins.Any(a => a.ApplicationUserId == userId),
                SD.Role_Doctor => context.Doctors.Any(a => a.ApplicationUserId == userId),
                SD.Role_Receptionist => context.Receptionists.Any(a => a.ApplicationUserId == userId),
                SD.Role_LabTechnician => context.LabTechnicians.Any(a => a.ApplicationUserId == userId),
                SD.Role_HeadLabTechnician => context.HeadLabTechnicians.Any(a => a.ApplicationUserId == userId),
                _ => false
            };
        }

    }
}
