using TradingApi.Manager.Storage.OrderSignal.Models;
using TradingApi.Manager.Storage.SignalDetector.Models;
using TradingApi.Repositories.Storages.SignalDetector.Models;

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
}
