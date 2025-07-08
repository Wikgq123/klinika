using Clinic.Enums;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    public class LabExam
    {
        [Key]
        public int LabExamId { get; set; }
        public string? DoctorsNotes { get; set; }
        public DateTime RequestDate { get; set; }
        public string? Result { get; set; }
        public DateTime? ExamDate { get; set; }
        public string? HeadLabNotes { get; set; }
        public DateTime? AcceptDate { get; set; }
        public ExamStatus Status { get; set; }
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public int? LabTechnicianId { get; set; }
        public LabTechnician? LabTechnician { get; set; }
        public int HeadLabTechnicianId { get; set; }
        public HeadLabTechnician HeadLabTechnician { get; set; }
        public string ExamSelectionId { get; set; }
        public ExamSelection ExamSelection { get; set; }
        public string? CancelationReason { get; set; }

    }
}
