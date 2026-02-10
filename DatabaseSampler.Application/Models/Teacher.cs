namespace DatabaseSampler.Application.Models;

public class Teacher
{
    public int Id { get; set; }
    public string FirstName { get; set; }   
    public string LastName { get; set; }   
    public string SpecialistSubject { get; set; }
    public DateTime Joined { get; set; }
    public DateTime Created { get; set; }
}
