using Clinic.Enums;
using Clinic.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<ExamSelection> ExamSelections { get; set; }
        public DbSet<LabExam> LabExams { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PhysicalExam> PhysicalExams { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Receptionist> Receptionists { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<LabTechnician> LabTechnicians { get; set; }
        public DbSet<HeadLabTechnician> HeadLabTechnicians { get; set; }

        public ApplicationDbContext(DbContextOptions options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<LabExam>()
                .HasOne(x => x.Appointment)
                .WithMany(x => x.LabExams)
                .HasForeignKey(x => x.AppointmentId);

            modelBuilder.Entity<PhysicalExam>()
                .HasOne(x => x.Appointment)
                .WithMany(x => x.PhysicalExams)
                .HasForeignKey(x => x.AppointmentId);


            modelBuilder.Entity<Patient>()
                .HasOne(x => x.Address)
                .WithMany(x => x.Patients) 
                .HasForeignKey(x => x.AddressId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<LabExam>()
                .HasOne(a => a.Appointment)
                .WithMany(p => p.LabExams)
                .HasForeignKey(a => a.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PhysicalExam>()
                .HasOne(a => a.Appointment)
                .WithMany(p => p.PhysicalExams)
                .HasForeignKey(a => a.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExamSelection>().Property(x => x.Type).HasConversion<string>();
            modelBuilder.Entity<Appointment>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<LabExam>().Property(x => x.Status).HasConversion<string>();

            modelBuilder.Entity<Patient>().HasData(
                new Patient { PatientId = 1, Name = "Gabriel", Surname = "Drabik", AddressId = 1, PESEL = "65110414558" },
                new Patient { PatientId = 2, Name = "Michael", Surname = "Brown", AddressId =2, PESEL = "57752850000" },
                new Patient { PatientId = 3, Name = "Emma", Surname = "Taylor", AddressId = 3, PESEL = "18204590000" },
                new Patient { PatientId = 4, Name = "John", Surname = "Brown", AddressId = 4, PESEL = "87673060000" },
                new Patient { PatientId = 5, Name = "Emma", Surname = "Taylor", AddressId = 5, PESEL = "99339650000" },
                new Patient { PatientId = 6, Name = "Anna", Surname = "Brown", AddressId = 6, PESEL = "73435320000" });

            modelBuilder.Entity<Address>().HasData(
                new Address{ AddressId = 1, City = "Gliwice", Street = "Akademicka", HomeNumber = "304" },
                new Address{ AddressId = 2, City = "Warsaw", Street = "Pine", HomeNumber = "26D" },
                new Address{ AddressId = 3, City = "Warsaw", Street = "Oak", HomeNumber = "25C" },
                new Address{ AddressId = 4, City = "Wroclaw", Street = "High", HomeNumber = "53A" },
                new Address{ AddressId = 5, City = "Poznan", Street = "Oak", HomeNumber = "42D" },
                new Address{ AddressId = 6, City = "Krakow", Street = "Pine", HomeNumber = "97B" }
                );
            modelBuilder.Entity<ExamSelection>().HasData(
                new ExamSelection { Shortcut = "Gen", Name = "General Checkup", Type = ExamType.Physical},
                new ExamSelection { Shortcut = "Blood", Name = "Blood Test", Type = ExamType.Lab},
                new ExamSelection { Shortcut = "XR", Name = "X-Ray", Type = ExamType.Lab}
                );
        }
    }
}
