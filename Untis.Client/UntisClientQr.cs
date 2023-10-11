namespace Untis.Client;

public class UntisClientQr: UntisClientBase
{
    private readonly string _qrcode;
    public UntisClientQr(string school, string baseurl, string qrcode, string identity) : base(school, baseurl, identity)
    {
        _qrcode = qrcode;
    }

    public override Task<bool> LoginAsync()
    {
        throw new NotImplementedException();
    }
}