using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TradingApi.Repositories.ArcadeDb.Models;

namespace TradingApi.Repositories.ArcadeDb;

public class ArcadeDbFactory : IArcadeDbFactory
{
    private readonly string _database;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    [SetsRequiredMembers]
    public ArcadeDbFactory(HttpClient httpClient, IOptions<ArcadeDbRepositoryOptions> options)
    {
        _database = options.Value.Database;
        _httpClient = httpClient;

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // add base url to httpclient
        ArgumentException.ThrowIfNullOrEmpty(options.Value.Host);
        _httpClient.BaseAddress = new Uri($"http://{options.Value.Host}:{options.Value.Port}/api/v1/");

        // add basic auth to httpclient
        ArgumentException.ThrowIfNullOrEmpty(options.Value.Username);
        ArgumentException.ThrowIfNullOrEmpty(options.Value.Password);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{options.Value.Username}:{options.Value.Password}")));

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public ArcadeDbConnection CreateConnection() => new(_httpClient, _database, _jsonSerializerOptions);

    public ArcadeDbTransactionConnection CreateTransactionConnection() => new(_httpClient, _database, _jsonSerializerOptions);
}
