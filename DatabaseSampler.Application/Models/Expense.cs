namespace DatabaseSampler.Application.Models;

public class Expense
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal Amount { get; set; }

    public string Frequency { get; set; }

    public DateTime StartDate { get; set; }

    public decimal MonthlyCost { get; set; }

    public DateTime Created { get; set; }
}
