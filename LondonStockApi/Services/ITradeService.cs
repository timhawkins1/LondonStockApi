using LondonStockApi.Models;

namespace LondonStockApi.Services;

public interface ITradeService
{
    public Task CreateAsync(Trade trade);
}
