using Microsoft.AspNetCore.Mvc.Rendering;

namespace Clinic.Models.ViewModels
{
    public class UpdateUserVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public string? Role { get; set; }
        public IEnumerable<SelectListItem>? RoleList { get; set; }
    }
}
