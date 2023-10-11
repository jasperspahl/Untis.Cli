using Untis.Client.Contracts;

namespace Untis.Client;

public class UntisClientBasic: UntisClientBase
{
    private readonly string _username;
    private readonly string _password;

    public UntisClientBasic(string school, string username, string password, string baseurl, string identity) : base(school, baseurl, identity)
    {
        _username = username;
        _password = password;
    }


    public override async Task<bool> LoginAsync()
    {
        try
        {
            SessionInformation =
                await PostAsync<SessionInformation>("authenticate", new
                    {
                        user = _username,
                        password = _password,
                        client = Id
                    }, false);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}