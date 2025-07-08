namespace Clinic.Models.ViewModels
{
    public class LabExamVM
    {

        public LabExam LabExam { get; set; }

        public ExamSelection? ExamSelection { get; set; }

        public Appointment? Appointment { get; set; }
    }
}
