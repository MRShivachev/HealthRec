using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthRec.Presentation.Models
{
    public class PatientRecordsViewModel
    {
        public string? PatientName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? MedicalRecordNumber { get; set; }

        public List<MedicalRecordViewModel> Records { get; set; } = new List<MedicalRecordViewModel>();
        public List<DoctorViewModel> Doctors { get; set; } = new List<DoctorViewModel>();
    }
}