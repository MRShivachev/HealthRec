using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthRec.Presentation.Models;

public class PatientCreateViewModel
{
    [Required(ErrorMessage = "First name is required")]
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Date of birth is required")]
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Please select a doctor")]
    [Display(Name = "Assign Doctor")]
    public Guid AssignedDoctorId { get; set; }

    public List<DoctorDropdownViewModel> AvailableDoctors { get; set; } = new List<DoctorDropdownViewModel>();
}