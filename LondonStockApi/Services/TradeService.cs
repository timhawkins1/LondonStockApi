using LondonStockApi.DataAccess;
using LondonStockApi.Models;

namespace LondonStockApi.Services;

public class TradeService : ITradeService
{
    private readonly ApplicationDbContext _dbContext;

    public TradeService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(Trade trade)
    {
        _dbContext.Add(trade);
        await _dbContext.SaveChangesAsync();
    }
}
