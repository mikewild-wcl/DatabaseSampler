namespace DatabaseSampler.Application.Models;

public static class SpecialistSubjects
{
    public const string Biology = "Biology";
    public const string Chemistry = "Chemistry";
    public const string ComputerScience = "Computer Science";
    public const string Maths = "Maths";
    public const string Physics = "Physics";

    public static IReadOnlyList<string> All =>
        [
            Biology,
            Chemistry,
            ComputerScience,
            Maths,
            Physics,
        ];
}
