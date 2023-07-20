using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace TradingApi.Repositories.Zero;

public class ZeroRepository : IZeroRepository
{
    private readonly string _awsPoolId;
    private readonly string _awsClientId;

    private readonly AmazonCognitoIdentityProviderClient _providerClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;

    private readonly string _clientId = string.Empty;

    [SetsRequiredMembers]
    public ZeroRepository(AmazonCognitoIdentityProviderClient providerClient, IConfiguration configuration, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _providerClient = providerClient;
        _httpContextAccessor = httpContextAccessor;

        _awsPoolId = configuration.GetValue<string>("ZeroAws:Pool") ?? throw new Exception("No Zero AWS Pool configuration");
        _awsClientId = configuration.GetValue<string>("ZeroAws:ClientId") ?? throw new Exception("No Zero AWS ClientId configuration");

        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://mein.finanzen-zero.net/api/");

        var token = httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type.Equals("token"))?.Value;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Add("Cookie", $"__i={token}");
        }

        _clientId = httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type.Equals("clientId"))?.Value ?? string.Empty;
    }

    public async Task<string?> LoginAsync(string username, string password)
    {

        var userPool = new CognitoUserPool(_awsPoolId, _awsClientId, _providerClient);
        var user = new CognitoUser(username, _awsClientId, userPool, _providerClient);

        var authResponse = await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest()
        {
            Password = password
        });


        if (_httpContextAccessor?.HttpContext?.Response?.Headers is { })
        {
            _httpContextAccessor.HttpContext.Response.Headers.TryAdd("zero-auth", authResponse?.AuthenticationResult?.IdToken);
        }

        return authResponse?.AuthenticationResult?.IdToken;
    }

    public async Task<object?> GetDepotPositionsAsync()
    {
        return await _httpClient.GetStringAsync($"trading/positions?customerId={_clientId}");
    }

    public async Task<object?> GetWatchlistAsync()
    {
        return await _httpClient.GetStringAsync($"wl");
    }
}
