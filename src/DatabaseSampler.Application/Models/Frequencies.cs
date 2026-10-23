namespace DatabaseSampler.Application.Models;

public static class Frequencies
{
    public const string Daily = "Daily";
    public const string Weekly = "Weekly";
    public const string Monthly = "Monthly";
    public const string Quarterly = "Quarterly";
    public const string Yearly = "Yearly";
    public const string Never = "Never";


    public static IReadOnlyList<string> All =>
        [
            Daily,
            Weekly,
            Monthly,
            Quarterly,
            Yearly,
            Never
        ];
}
