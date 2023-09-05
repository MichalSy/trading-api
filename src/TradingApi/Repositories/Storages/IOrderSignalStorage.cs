using TradingApi.Manager.Storage.OrderSignal.Models;
using TradingApi.Repositories.Storages.Models;
using TradingApi.Repositories.ZeroRealtime.Models;

namespace TradingApi.Repositories.Storages;

public interface IOrderSignalStorage
{
    Task<IEnumerable<OrderSignalEntityDBO>?> GetOrderSignalsAsync();
    Task<OrderSignalEntityDBO> CreateOrUpdateOrderSignalAsync(OrderSignalEntityDBO orderSignal);
    Task<OrderSignalEntityDBO?> GetOrderSignalAsync(Guid id);
}