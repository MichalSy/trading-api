using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace TradingApi.Authentication;

public class ZeroTradeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public ZeroTradeAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authValue = Request.Headers["zero-auth"].ToString();

        if (string.IsNullOrEmpty(authValue))
            return Task.FromResult(AuthenticateResult.Fail("missing auth header"));

        var token = new JwtSecurityToken(jwtEncodedString: authValue);
        string accountContent = token?.Claims?.FirstOrDefault(c => c.Type.Equals("prm"))?.Value ?? string.Empty;
        if (string.IsNullOrEmpty(accountContent))
            return Task.FromResult(AuthenticateResult.Fail("incorrect jwt token"));

        var data = JsonSerializer.Deserialize<Dictionary<string, ZeroTradeAuthAccountData>>(accountContent);
        if (data is null || data.Count == 0)
            return Task.FromResult(AuthenticateResult.Fail("incorrect jwt data"));

        var accData = data.First().Value;

        return Task.FromResult(AuthenticateResult.Success(
            new AuthenticationTicket(
                new ClaimsPrincipal(
                    new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Name, "ZeroTrade"),
                                new Claim("token", authValue),
                                new Claim("clientId", data.Keys.First()),
                                new Claim("firstName", accData.firstName),
                                new Claim("lastName", accData.lastName),
                            }, "ZeroTrade")
                        ), Scheme.Name)
            )
        );
    }
}
