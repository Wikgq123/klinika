using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    public class Receptionist
    {
        [Key]
        public int ReceptionistId { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
