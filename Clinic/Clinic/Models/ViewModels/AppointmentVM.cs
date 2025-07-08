namespace Clinic.Models.ViewModels
{
    public class AppointmentVM
    {
        public Appointment Appointment { get; set; }
        public List<Patient>? Patients { get; set; }
        public List<Doctor>? Doctors { get; set; }
    }
}
