using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    public class LabTechnician
    {
        [Key]
        public int LabTechnicianId { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
