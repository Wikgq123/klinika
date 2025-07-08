using Clinic.Enums;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        [Required]
        public string Description { get; set; }
        public string? Diagnosis { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTime RegistrationDate { get; set; }
        [Required(ErrorMessage = "Appointment Date is required")]
        public DateTime? AppointmentDate { get; set; }
        [Required(ErrorMessage = "Doctor is required")]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
        [Required(ErrorMessage = "Patient is required")]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int ReceptionistId { get; set; }
        public Receptionist? Receptionist { get; set; }
        public ICollection<LabExam>? LabExams { get; set; }
        public ICollection<PhysicalExam>? PhysicalExams { get; set; }

    }
}
