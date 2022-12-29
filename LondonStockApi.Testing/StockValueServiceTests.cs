using LondonStockApi.DataAccess;
using LondonStockApi.Models;
using LondonStockApi.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LondonStockApi.Testing;

public class StockValueServiceTests
{
    private DbContextOptions _dbContextOptions;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Construct the DB context options using an in-memory database connection.
        var sqliteConnection = new SqliteConnection("DataSource=:memory:");
        sqliteConnection.Open();
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(sqliteConnection)
            .Options;

        // Apply the database schema.
        using var context = new ApplicationDbContext(_dbContextOptions);
        context.Database.EnsureCreated();

        // Seed the database with some trades.
        using var dbContext = new ApplicationDbContext(_dbContextOptions);
        var tradeService = new TradeService(dbContext);
        await tradeService.CreateAsync(new Trade(1, "AAPL", 101, true, 100, "BROKER1"));
        await tradeService.CreateAsync(new Trade(2, "MSFT", 201, false, 200, "BROKER2"));
        await tradeService.CreateAsync(new Trade(3, "AAPL", 105, true, 100, "BROKER2"));
        await tradeService.CreateAsync(new Trade(4, "MSFT", 203, false, 200, "BROKER3"));
        await tradeService.CreateAsync(new Trade(5, "GOOGL", 301, true, 300, "BROKER1"));
    }

    [Test]
    public async Task GetAllAsync_ReturnsList()
    {
        using var context = new ApplicationDbContext(_dbContextOptions);
        var stockValueService = new StockValueService(context);
        var stockValues = await stockValueService.GetAllAsync();
        Assert.That(stockValues, Has.Count.EqualTo(3));
        var stockValueDict = stockValues.ToDictionary(x => x.Ticker);
        Assert.Multiple(() =>
        {
            Assert.That(stockValueDict["AAPL"].Value, Is.EqualTo(103));
            Assert.That(stockValueDict["MSFT"].Value, Is.EqualTo(202));
            Assert.That(stockValueDict["GOOGL"].Value, Is.EqualTo(301));
        });
    }

    [Test]
    public async Task GetOneAsync_GivenValidTicker_ReturnsOne()
    {
        using var context = new ApplicationDbContext(_dbContextOptions);
        var stockValueService = new StockValueService(context);
        var stockValue = await stockValueService.GetOneAsync("AAPL");
        Assert.That(stockValue.Value, Is.EqualTo(103));
    }

    [Test]
    public void GetOneAsync_GivenInvalidTicker_ThrowsArgumentException()
    {
        using var context = new ApplicationDbContext(_dbContextOptions);
        var stockValueService = new StockValueService(context);
        Assert.ThrowsAsync<ArgumentException>(() => stockValueService.GetOneAsync("META"));
    }

    [Test]
    public async Task GetRangeAsync_GivenAllValidTickers_ReturnsList()
    {
        using var context = new ApplicationDbContext(_dbContextOptions);
        var stockValueService = new StockValueService(context);
        var stockValues = await stockValueService.GetRangeAsync(new[] { "AAPL", "GOOGL" });
        Assert.That(stockValues, Has.Count.EqualTo(2));
        var stockValueDict = stockValues.ToDictionary(x => x.Ticker);
        Assert.Multiple(() =>
        {
            Assert.That(stockValueDict["AAPL"].Value, Is.EqualTo(103));
            Assert.That(stockValueDict["GOOGL"].Value, Is.EqualTo(301));
        });
    }

    [Test]
    public async Task GetRangeAsync_GivenMixOfValidAndInvalidTickers_ReturnsList()
    {
        using var context = new ApplicationDbContext(_dbContextOptions);
        var stockValueService = new StockValueService(context);
        var stockValues = await stockValueService.GetRangeAsync(new[] { "AAPL", "GOOGL", "META" });
        Assert.That(stockValues, Has.Count.EqualTo(2));
        var stockValueDict = stockValues.ToDictionary(x => x.Ticker);
        Assert.Multiple(() =>
        {
            Assert.That(stockValueDict["AAPL"].Value, Is.EqualTo(103));
            Assert.That(stockValueDict["GOOGL"].Value, Is.EqualTo(301));
        });
    }

    [Test]
    public async Task GetRangeAsync_GivenAllInvalidTickers_ReturnsEmptyList()
    {
        using var context = new ApplicationDbContext(_dbContextOptions);
        var stockValueService = new StockValueService(context);
        var stockValues = await stockValueService.GetRangeAsync(new[] { "TSLA", "META" });
        Assert.That(stockValues, Has.Count.EqualTo(0));
    }
}
