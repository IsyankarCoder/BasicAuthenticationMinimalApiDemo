using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public class BasicAuth
            : AuthenticationHandler<AuthenticationSchemeOptions>
{

    public BasicAuth(IOptionsMonitor<AuthenticationSchemeOptions> options,
                     ILoggerFactory logger,
                     UrlEncoder encoder
                      )
     : base(options, logger, encoder)
    {

    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
        }

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? "");
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2, StringSplitOptions.None);
            var userName = credentials[0];
            var passWord = credentials[1];

            if (userName != "admin" || passWord != "pas123")
            {
               return Task.FromResult(AuthenticateResult.Fail("Invalid UserName or PassWord"));
            }

            var claims = new[]
            {
               new Claim(ClaimTypes.NameIdentifier,userName),
               new Claim(ClaimTypes.Name,userName),
               new Claim(ClaimTypes.Role,"Admin")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));


        }
        catch (System.Exception ex)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Handler"));
        }
    }
}



    
