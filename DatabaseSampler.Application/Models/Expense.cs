using System.Text.Json.Serialization;

namespace DatabaseSampler.Application.Models;

public class Expense
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("frequency")]
    public string Frequency { get; set; }

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("monthlyCost")]
    public decimal MonthlyCost { get; set; }

    [JsonPropertyName("created")]
    public DateTime Created { get; set; }
}
