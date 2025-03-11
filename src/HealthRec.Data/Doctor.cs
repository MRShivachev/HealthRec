namespace HealthRec.Data.Entities;

public enum Specialisation
{
    Cardiologist,
    Neurologist,
    Onkologist,
    Dermatologist,
    Gastroenterologist,
}
public class Doctor : ApplicationUser
{
   public Specialisation specialisation {get;set;}
   
}