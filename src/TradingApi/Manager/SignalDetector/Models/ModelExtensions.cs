using TradingApi.Manager.Storage.OrderSignal.Models;
using TradingApi.Manager.Storage.SignalDetector.Models;
using TradingApi.Repositories.Storages.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Manager.OrderSignalDetector.Models;

public static class ModelExtensions
{
    public static SignalDetectorJob ToDTO(this SignalDetectorEntityDBO dbo)
    {
        return new SignalDetectorJob
        {
            Id = dbo.Id,
            Isin = dbo.Isin,
            DetectorName = dbo.DetectorName,
            DetectorSettings = dbo.DetectorSettings,
            OrderSignalSettings = new OrderSignalSettings
            { 
                BuySettings = new OrderSignalBuySettings
                {
                    CoolDownAfterLastSellInSecs = dbo.OrderSignalSettings.BuySettings.CoolDownAfterLastSellInSecs,
                    RoundUpValueInEur = dbo.OrderSignalSettings.BuySettings.RoundUpValueInEur,
                    StockCount = dbo.OrderSignalSettings.BuySettings.StockCount,
                    ValueInEur = dbo.OrderSignalSettings.BuySettings.ValueInEur
                }, 
                SellSettings = new OrderSignalSellSettings
                {
                    DifferenceNegativeInPercent = dbo.OrderSignalSettings.SellSettings.DifferenceNegativeInPercent,
                    DifferencePositiveInPercent = dbo.OrderSignalSettings.SellSettings.DifferencePositiveInPercent,
                    MaxDuration = dbo.OrderSignalSettings.SellSettings.MaxDuration
                }
            }
        };
    }

    public static SignalDetectorEntityDBO ToDBO(this SignalDetectorJob dto)
    {
        return new SignalDetectorEntityDBO
        {
            Id = dto.Id,
            Isin = dto.Isin,
            DetectorName = dto.DetectorName,
            DetectorSettings = dto.DetectorSettings,
            OrderSignalSettings = new OrderSignalSettingsDBO
            {
                BuySettings = new OrderSignalBuySettingsDBO
                {
                    CoolDownAfterLastSellInSecs = dto.OrderSignalSettings.BuySettings.CoolDownAfterLastSellInSecs,
                    RoundUpValueInEur = dto.OrderSignalSettings.BuySettings.RoundUpValueInEur,
                    StockCount = dto.OrderSignalSettings.BuySettings.StockCount,
                    ValueInEur = dto.OrderSignalSettings.BuySettings.ValueInEur
                },
                SellSettings = new OrderSignalSellSettingsDBO
                {
                    DifferenceNegativeInPercent = dto.OrderSignalSettings.SellSettings.DifferenceNegativeInPercent,
                    DifferencePositiveInPercent = dto.OrderSignalSettings.SellSettings.DifferencePositiveInPercent,
                    MaxDuration = dto.OrderSignalSettings.SellSettings.MaxDuration
                }
            }
        };
    }

    public static RealtimeQuoteDBO ToDBO(this RealtimeQuote dto)
    {
        return new RealtimeQuoteDBO
        {
            Ask = dto.Ask,
            Bid = dto.Bid,
            Isin = dto.Isin,
            Timestamp = dto.Timestamp
        };
    }

    public static RealtimeQuote ToDTO(this RealtimeQuoteDBO dbo)
    {
        return new RealtimeQuote
        {
            Ask = dbo.Ask,
            Bid = dbo.Bid,
            Isin = dbo.Isin,
            Timestamp = dbo.Timestamp
        };
    }

    public static OrderSignalEntityDBO ToDBO(this OrderSignalJob dto)
    {
        return new OrderSignalEntityDBO
        {
            Id = dto.Id,
            CreatedDate = dto.CreatedDate,
            DetectorJobId = dto.DetectorJobId,
            Isin = dto.Isin,
            StockCount = dto.StockCount,
            BuySettings = new OrderSignalBuySettingsDBO
            {
                CoolDownAfterLastSellInSecs = dto.BuySettings.CoolDownAfterLastSellInSecs,
                RoundUpValueInEur = dto.BuySettings.RoundUpValueInEur,
                StockCount = dto.BuySettings.StockCount,
                ValueInEur = dto.BuySettings.ValueInEur
            },
            BuyQuote = dto.BuyQuote!.ToDBO(),
            SellSettings = new OrderSignalSellSettingsDBO
            {
                DifferenceNegativeInPercent = dto.SellSettings.DifferenceNegativeInPercent,
                DifferencePositiveInPercent = dto.SellSettings.DifferencePositiveInPercent,
                MaxDuration = dto.SellSettings.MaxDuration
            },
            SellQuote = dto.SellQuote?.ToDBO(),
            TotalBuyValueInEur = dto.TotalBuyValueInEur,
            TotalSellValueInEur = dto.TotalSellValueInEur,
            TotalProfitInEur = dto.TotalProfitInEur,
        };
    }

    public static OrderSignalJob ToDTO(this OrderSignalEntityDBO dbo)
    {
        return new OrderSignalJob
        {
            Id = dbo.Id,
            CreatedDate = dbo.CreatedDate,
            DetectorJobId = dbo.DetectorJobId,
            Isin = dbo.Isin,
            StockCount = dbo.StockCount,
            BuySettings = new OrderSignalBuySettings
            {
                CoolDownAfterLastSellInSecs = dbo.BuySettings.CoolDownAfterLastSellInSecs,
                RoundUpValueInEur = dbo.BuySettings.RoundUpValueInEur,
                StockCount = dbo.BuySettings.StockCount,
                ValueInEur = dbo.BuySettings.ValueInEur
            },
            BuyQuote = dbo.BuyQuote.ToDTO(),
            SellSettings = new OrderSignalSellSettings
            {
                DifferenceNegativeInPercent = dbo.SellSettings.DifferenceNegativeInPercent,
                DifferencePositiveInPercent = dbo.SellSettings.DifferencePositiveInPercent,
                MaxDuration = dbo.SellSettings.MaxDuration
            },
            SellQuote = dbo.SellQuote?.ToDTO(),
            TotalBuyValueInEur = dbo.TotalBuyValueInEur,
            TotalSellValueInEur = dbo.TotalSellValueInEur,
            TotalProfitInEur = dbo.TotalProfitInEur,
        };
    }
}
