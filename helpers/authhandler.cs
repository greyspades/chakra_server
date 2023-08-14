using System.Security.Claims;
using System.Text.Encodings.Web;
using AES;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AuthHandler;

public class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationOptions>
{
    IConfiguration _config;
    public CustomAuthenticationHandler(IConfiguration config, IOptionsMonitor<CustomAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _config = config;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        //* checks if there is an auth token
        if (!Request.Headers.TryGetValue("Auth", out var tokenValue))
        {
            return AuthenticateResult.Fail("Missing custom token");
        }

        //* Validates the token
        if (!IsValidToken(tokenValue))
        {  
            return AuthenticateResult.Fail("Invalid custom token");
        }
        else {
            //*sets claims for the application if necessary
        var claims = new[] { new Claim(ClaimTypes.Name, "username") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
        }
    }

    // Add your token validation logic here
    private bool IsValidToken(string token)
    {   var key = _config.GetValue<string>("Encryption:Key");
        var iv = _config.GetValue<string>("Encryption:Iv");

        var decryptedToken = AEShandler.Decrypt(token, key: key, iv);
        if( decryptedToken == _config.GetValue<string>("Auth:Token")) {
            return true;
        } else {
            return false;
        }
    }
}

public static class CustomAuthenticationExtensions
{
    public static AuthenticationBuilder AddCustomAuthentication(this AuthenticationBuilder builder)
    {
        return builder.AddScheme<CustomAuthenticationOptions, CustomAuthenticationHandler>(CustomAuthenticationOptions.AuthenticationScheme, null);
    }
}
