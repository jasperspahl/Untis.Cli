using System.Text.Json.Serialization;

namespace Untis.Client.Contracts;

public class SessionInformation
{
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }
    [JsonPropertyName("personId")]
    public int? PersonId { get; set; }
    [JsonPropertyName("klassenId")]
    public int? KlassenId { get; set; }
    [JsonPropertyName("personType")]
    public int? PersonType { get; set; }
    [JsonPropertyName("jwt_token")]
    public string? JwtToken { get; set; }
}