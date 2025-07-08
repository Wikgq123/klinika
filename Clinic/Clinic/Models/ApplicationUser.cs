using Microsoft.AspNetCore.Identity;

namespace Clinic.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public Doctor? Doctor { get; set; }
        public Receptionist? Receptionist { get; set; }
        public Admin? Admin { get; set; }
        public LabTechnician? LabTechnician { get; set; }
        public HeadLabTechnician? HeadLabTechnician { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
