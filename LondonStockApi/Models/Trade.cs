namespace LondonStockApi.Models;

public record Trade(int Id, string Ticker, decimal Price, bool BuySell, decimal NumberOfShares, string BrokerId);
