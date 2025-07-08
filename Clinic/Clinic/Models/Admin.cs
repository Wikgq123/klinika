using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
