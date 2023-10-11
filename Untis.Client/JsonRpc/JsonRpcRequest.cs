using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Untis.Client.JsonRpc;

public class JsonRpcRequest<T>
{
    public JsonRpcRequest(string method, T @params, string? id)
    {
        Method = method;
        Params = @params;
        Id = id;
    }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; }
    
    [JsonPropertyName("params")]
    public T Params { get; set; }

    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; } = "2.0";
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
    
    public JsonContent ToJsonContent()
    {
        return JsonContent.Create(this);
    }
}