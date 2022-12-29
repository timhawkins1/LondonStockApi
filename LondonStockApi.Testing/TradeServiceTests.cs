using LondonStockApi.DataAccess;
using LondonStockApi.Models;
using LondonStockApi.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LondonStockApi.Testing;

public class TradeServiceTests
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
    }

    [Test]
    public async Task CreateAsync_GivenValidTrade_WritesToDatabase()
    {
        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var tradeService = new TradeService(context);
            var trade = new Trade(1, "AAPL", 101, true, 100, "BROKER1");
            await tradeService.CreateAsync(trade);

            Assert.That(context.Trades.ToList(), Has.Count.EqualTo(1));
        }

        using (var context = new ApplicationDbContext(_dbContextOptions))
        {
            var tradeService = new TradeService(context);
            var trade = new Trade(2, "MSFT", 201, false, 200, "BROKER2");
            await tradeService.CreateAsync(trade);

            Assert.That(context.Trades.ToList(), Has.Count.EqualTo(2));
        }
    }

    [Test]
    public void CreateAsync_GivenInvalidTrade_ThrowsArgumentException()
    {
        using var context = new ApplicationDbContext(_dbContextOptions);
        var tradeService = new TradeService(context);
        var trade = new Trade(3, "AAPL", -101, true, 100, "BROKER1");
        Assert.ThrowsAsync<ArgumentException>(() => tradeService.CreateAsync(trade));
    }
}

