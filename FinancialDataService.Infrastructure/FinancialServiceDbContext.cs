using Microsoft.EntityFrameworkCore;

namespace FinancialDataService.Infrastructure
{
    public class FinancialServiceDbContext : DbContext
    {
        public FinancialServiceDbContext(DbContextOptions<FinancialServiceDbContext> options)
            : base(options)
        {
        }
    }
}