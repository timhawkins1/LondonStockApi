using LondonStockApi.DataAccess;
using LondonStockApi.Models;
using System.Text;

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
        ValidateTrade(trade);
        _dbContext.Add(trade);
        await _dbContext.SaveChangesAsync();
    }

    private static void ValidateTrade(Trade trade)
    {
        var error = new StringBuilder();

        if (string.IsNullOrWhiteSpace(trade.Ticker))
        {
            error.AppendLine("Ticker must not be blank.");
        }

        if (trade.Price <= 0)
        {
            error.AppendLine("Price must be greater than zero.");
        }

        if (trade.NumberOfShares <= 0)
        {
            error.AppendLine("Number of shares must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(trade.BrokerId))
        {
            error.AppendLine("Broker Id must not be blank.");
        }

        if (error.Length > 0)
        {
            throw new ArgumentException(error.ToString());
        }
    }
}
