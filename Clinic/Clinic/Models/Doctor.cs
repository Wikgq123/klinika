using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    public class Doctor
    {
        [Key]
        public int? DoctorId { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public string? NPWZ { get; set; }
    }
}
