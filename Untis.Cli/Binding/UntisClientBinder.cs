using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System.CommandLine;
using System.CommandLine.Binding;
using Untis.Client;
using Untis.Cli.Helpers;

namespace Untis.Cli.Binding;

public sealed class SchoolOptions
{
    public const string SectionKey = "School";
    public required string SchoolName { get; set; }
    public required string BaseUrl { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? PasswordCmd { get; set; }
    public string? Key { get; set; }
    public string? QrCode { get; set; }
}

public class UntisClientBinder: BinderBase<IUntisClient>
{
    private SchoolOptions? _options;
    private readonly Option<string> _schoolOption = new(new []{"-s", "--school"}, "The name of the school");
    private readonly Option<string> _baseUrlOption = new(new []{"-b", "--baseurl"}, "The base url of the untis server");
    private readonly Option<string> _usernameOption = new(new [] {"-u", "--username"}, "The username to login with");
    private readonly Option<string> _passwordOption = new(new [] {"-p", "--password"}, "The password to login with");
    private readonly Option<string> _passwordCmdOption = new("--password-cmd", "The command to get the password from");
    
    public UntisClientBinder(Command cmd)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<Program>()
            .Build();
        _options = config.GetSection(SchoolOptions.SectionKey).Get<SchoolOptions>();
        cmd.Add(_schoolOption);
        cmd.Add(_baseUrlOption);
        cmd.Add(_usernameOption);
        cmd.Add(_passwordOption);
        cmd.Add(_passwordCmdOption);
    }
    protected override IUntisClient GetBoundValue(BindingContext bindingContext)
    {
        _options ??= new SchoolOptions
        {
            SchoolName = bindingContext.ParseResult.GetValueForOption(_schoolOption)!,
            BaseUrl = bindingContext.ParseResult.GetValueForOption(_baseUrlOption)!,
        };
        
        var qrCode = bindingContext.ParseResult.GetValueForOption(_passwordOption) ?? _options.QrCode;
        if (qrCode != null)
        {
            return new UntisClientQr(_options.SchoolName, _options.BaseUrl, qrCode, "Untis.Cli");
        }
        
        // if (_options.Key != null)
        // {
        //     return new UntisClientKey(_options.SchoolName, _options.Key, _options.BaseUrl, "Untis.Cli");
        // }
        
        var username = bindingContext.ParseResult.GetValueForOption(_usernameOption) ?? _options.Username;
        username ??= AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]username[/]?")
                .PromptStyle("red")
        );
        var password = bindingContext.ParseResult.GetValueForOption(_passwordOption) ?? _options.Password;
        var passwordCmd = bindingContext.ParseResult.GetValueForOption(_passwordCmdOption) ?? _options.PasswordCmd;
        if (passwordCmd != null )
        {
            password = ShellHelper.RunCommand(passwordCmd);
        }

        password ??= AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]password[/]?")
                .PromptStyle("red")
                .Secret()
        );

        return new UntisClientBasic(_options.SchoolName, username, password, _options.BaseUrl, "Untis.Cli");
    }
}