using System.Text.Json.Serialization;

namespace Untis.Client.Contracts;

public class ShortData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("longname")]
    public required string LongName { get; set; }

    public override string ToString()
    {
        return $"{Name} ({LongName})";
    }
}