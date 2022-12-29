using LondonStockApi.Models;

namespace LondonStockApi.Services;

public interface IStockValueService
{
    public Task<List<StockValue>> GetAllAsync();

    public Task<StockValue> GetOneAsync(string ticker);

    public Task<List<StockValue>> GetRangeAsync(IEnumerable<string> tickers);
}
