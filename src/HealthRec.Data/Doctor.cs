namespace HealthRec.Data.Entities;

public enum Specialisation
{
    Cardiologist = 1,
    Neurologist = 2,
    Onkologist = 3,
    Dermatologist = 4,
    Gastroenterologist = 5,
}

public class Doctor : ApplicationUser
{
   public Specialisation Specialisation { get; set; }
}