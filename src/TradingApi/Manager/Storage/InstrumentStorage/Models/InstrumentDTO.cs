namespace TradingApi.Manager.Storage.InstrumentStorage.Models;

public sealed class InstrumentDTO
{
    public required string Isin { get; init; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
