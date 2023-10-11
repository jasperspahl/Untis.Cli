using System.Text.Json.Serialization;

namespace Untis.Client.JsonRpc;

public class JsonRpcResult<T>
{
    [JsonPropertyName("jsonrpc")]
    public string Jsonrpc { get; set; } = "2.0";
    
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("result")]
    public required T Result { get; set; }
}