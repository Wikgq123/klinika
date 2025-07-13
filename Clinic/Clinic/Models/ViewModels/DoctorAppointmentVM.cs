﻿using Clinic.Models;
using Clinic.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Models.ViewModels
{
    public class DoctorAppointmentVM
    {
        public Appointment Appointment { get; set; }

        public IEnumerable<ExamSelection> ExamSelections { get; set; }

        // — Historia poprzednich wizyt —
        public IEnumerable<Appointment> PreviousVisits { get; set; }

        [Display(Name = "Wybierz badanie laboratoryjne")]
        public string? NewLabExamType { get; set; }

        [Display(Name = "Wybierz badanie fizykalne")]
        public string? NewPhysicalExamType { get; set; }

        [Display(Name = "Notatki do badania fizykalnego")]
        public string? NewPhysicalExamNotes { get; set; }

        // pola do edycji statusu istniejących badań laboratoryjnych
        public List<int>? LabExamIds { get; set; }
        public List<ExamStatus>? LabExamStatuses { get; set; }
    }
}
