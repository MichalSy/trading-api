namespace TradingApi.Manager.TradingStorage.Models;

public sealed class InstrumentDTO
{
    public required string Isin { get; init; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
