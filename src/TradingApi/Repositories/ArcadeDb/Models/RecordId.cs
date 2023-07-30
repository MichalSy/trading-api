using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TradingApi.Repositories.ArcadeDb.Models;


[JsonConverter(typeof(RecordIdConverter))]
public readonly partial struct RecordId
{
    [GeneratedRegex(@"^#\d+:\d+$", RegexOptions.Compiled)]
    private static partial Regex _isValidRegex();

    private readonly string _recordId;

    public RecordId(string? recordId)
    {
        if (IsRecordId(recordId) == false) throw new ArgumentException($"{recordId} is not a valid Record Id", nameof(recordId));
        this._recordId = recordId!;
    }

    public static implicit operator RecordId(string recordId) => new(recordId);

    public static implicit operator string(RecordId recordId) => recordId._recordId;

    public override string ToString() => _recordId;

    public static bool IsRecordId(string? candidate) => _isValidRegex().IsMatch(candidate ?? string.Empty);


}

internal class RecordIdConverter : JsonConverter<RecordId>
{
    public override RecordId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new(reader.GetString());

    public override void Write(Utf8JsonWriter writer, RecordId value, JsonSerializerOptions options) => writer.WriteStringValue(value);
}
