using FinancialDataService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FinancialDataService.Infrastructure
{
    public class FinancialServiceDbContext : DbContext
    {
        public FinancialServiceDbContext(DbContextOptions<FinancialServiceDbContext> options)
            : base(options)
        {
        }

        public DbSet<FinancialInstrument> FinancialInstruments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}