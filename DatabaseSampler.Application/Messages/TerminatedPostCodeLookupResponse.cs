using System.Text.Json.Serialization;

namespace DatabaseSampler.Application.Messages;

public class TerminatedPostcodeLookupResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
    [JsonPropertyName("result")]
    public TerminatedPostcodeLookupResultData Result { get; set; }
}