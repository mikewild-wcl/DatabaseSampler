using System.Text.Json.Serialization;

namespace DatabaseSampler.Application.Messages;

public class PostcodeLookupResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
    [JsonPropertyName("result")]
    public PostcodeLookupResultData Result { get; set; }
}