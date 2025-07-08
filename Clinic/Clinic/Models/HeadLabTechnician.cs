using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    public class HeadLabTechnician
    {
        [Key]
        public int HeadLabTechnicianId { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
