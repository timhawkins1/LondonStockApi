using LondonStockApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LondonStockApi.DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Trade> Trades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trade>()
            .HasIndex(x => x.Ticker)
            .IncludeProperties(x => new { x.Price });
    }
}
