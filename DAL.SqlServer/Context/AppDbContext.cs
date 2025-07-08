using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DAL.SqlServer.Context;

public class AppDbContext : DbContext
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<TableSession> TableSessions { get; set; }
    public DbSet<SessionProduct> SessionProducts{ get; set; }
    public DbSet<Expenses> Expenses{ get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<TablesPrice> TablesPrices { get; set; }
    public DbSet<User> Users { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
