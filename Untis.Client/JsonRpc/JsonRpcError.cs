using System.Text.Json.Serialization;

namespace Untis.Client.JsonRpc;

public class JsonRpcError
{
    [JsonPropertyName("jsonrpc")]
    public string Jsonrpc { get; set; } = "2.0";
    
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("error")]
    public required JsonRpcErrorObject Error { get; set; }
    
}

public class JsonRpcErrorObject
{
    [JsonPropertyName("code")]
    public int Code { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("data")]
    public object? Data { get; set; }
}