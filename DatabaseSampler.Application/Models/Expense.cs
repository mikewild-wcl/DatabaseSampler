using System.Text.Json.Serialization;

namespace DatabaseSampler.Application.Models;

public class Expense
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("amount")]
    public double Amount { get; set; }

    [JsonPropertyName("frequency")]
    public string Frequency { get; set; }

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("monthlyCost")]
    public double MonthlyCost { get; set; }
}
