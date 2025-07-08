using Clinic.Enums;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    public class ExamSelection
    {
        [Key]
        [MaxLength(10)]
        public string Shortcut { get; set; }
        public ExamType Type { get; set; }
        public string? Name { get; set; }
    }
}
