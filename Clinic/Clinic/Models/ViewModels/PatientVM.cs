namespace Clinic.Models.ViewModels
{
    public class PatientVM
    {
        public Patient Patient { get; set; }

        public List<Address>? Addresses { get; set; }
    }
}
