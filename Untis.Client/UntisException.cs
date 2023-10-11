using Untis.Client.JsonRpc;

namespace Untis.Client;

public class UntisException : Exception
{
    private JsonRpcError JsonRpcError { get; set; }

    public UntisException(JsonRpcError jsonRpcError)
    {
        JsonRpcError = jsonRpcError;
    }
    
    public JsonRpcErrorObject Error => JsonRpcError.Error;
    public int Code => Error.Code;
    
    public override string Message => $"UntisException was thrown: Code {Code} {Error.Message} {Error.Data}";

}