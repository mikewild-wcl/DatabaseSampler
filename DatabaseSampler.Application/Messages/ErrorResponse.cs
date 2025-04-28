using System.Text.Json.Serialization;

namespace DatabaseSampler.Application.Messages;

public class ErrorResponse
{
    [JsonPropertyName("error")]
    public string Error { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; }
}
