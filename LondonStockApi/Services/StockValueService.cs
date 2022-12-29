using LondonStockApi.DataAccess;
using LondonStockApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LondonStockApi.Services;

public class StockValueService : IStockValueService
{
    private readonly ApplicationDbContext _dbContext;

    public StockValueService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<StockValue>> GetAllAsync()
    {
        var tradesByTicker = await _dbContext.Trades.Select(x => new { x.Ticker, x.Price })
            .GroupBy(x => x.Ticker).ToListAsync();
        return tradesByTicker.Select(x => new StockValue(x.Key, x.Average(y => y.Price))).ToList();
    }

    public async Task<StockValue> GetOneAsync(string ticker)
    {
        var trades = await _dbContext.Trades.Where(x => x.Ticker == ticker).ToListAsync();
        if (!trades.Any())
        {
            throw new ArgumentException($"No trades found for ticker: {ticker}");
        }
        return new StockValue(ticker, trades.Average(x => x.Price));
    }

    public async Task<List<StockValue>> GetRangeAsync(IEnumerable<string> tickers)
    {
        var tradesByTicker = await _dbContext.Trades.Where(x => tickers.Contains(x.Ticker))
            .Select(x => new { x.Ticker, x.Price }).GroupBy(x => x.Ticker).ToListAsync();
        return tradesByTicker.Select(x => new StockValue(x.Key, x.Average(y => y.Price))).ToList();
    }
}
