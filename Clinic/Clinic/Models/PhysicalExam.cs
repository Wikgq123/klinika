using System.ComponentModel.DataAnnotations;

namespace Clinic.Models
{
    public class PhysicalExam
    {
        [Key]
        public int PhisicalExamId { get; set; }
        public string Result { get; set; }
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public string ExamSelectionId { get; set; }
        public ExamSelection ExamSelection { get; set; }
    }
}
    