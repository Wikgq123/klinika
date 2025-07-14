using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.ExamRecords
{
    public class PhysicalExam
    {
        [Key]
        public int Id { get; set; }
        public string ExamType { get; set; }
        public string Notes { get; set; }
        public int PatientId { get; set; }
        public string DoctorId { get; set; }
        public DateTime Date { get; set; }
    }
}
