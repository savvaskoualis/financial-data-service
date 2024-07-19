using FinancialDataService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialDataService.Infrastructure.EntityConfigurations
{
    public class FinancialInstrumentConfiguration : IEntityTypeConfiguration<FinancialInstrument>
    {
        public void Configure(EntityTypeBuilder<FinancialInstrument> builder)
        {
            builder.HasKey(fi => fi.Id);
            builder.Property(fi => fi.Symbol)
                .IsRequired()
                .HasMaxLength(10);
        }
    }
}