using System.Text.Json;
using System.Text;
using TradingApi.Repositories.ArcadeDb.Models;

namespace TradingApi.Repositories.ArcadeDb;

public class ArcadeDbConnection
{
    private const string _sessionHeaderName = "arcadedb-session-id";

    private readonly HttpClient _httpClient;
    private readonly string _database;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    [SetsRequiredMembers]
    public ArcadeDbConnection(HttpClient httpClient, string database, JsonSerializerOptions jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _database = database;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task<HttpResponseMessage> ExecuteCommandAsync(HttpMethod httpMethod, string command, CommandPayload commandPayload, string? sessionId = null)
    {
        using StringContent jsonContent = new(
            JsonSerializer.Serialize(commandPayload, _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json"
        );

        using var request = new HttpRequestMessage(httpMethod, $"{command}/{_database}")
        {
            Content = jsonContent
        };

        if (!string.IsNullOrEmpty(sessionId))
            request.Headers.Add(_sessionHeaderName, sessionId);

        var response = await _httpClient.SendAsync(request);


        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ArcadeDbErrorResponse>();
            throw new Exception($"{errorResponse!.Error}:\n{errorResponse!.Detail}");
        }

        return response;
    }

    public async Task<HttpResponseMessage> ExecuteSqlScriptAsync(string sqlScript, object? parameters = null)
    {
        return await ExecuteCommandAsync(HttpMethod.Post, "command", new CommandPayload
        {
            Command = sqlScript,
            Parameters = parameters,
            Language = QueryLanguage.SqlScript
        });
    }
}
