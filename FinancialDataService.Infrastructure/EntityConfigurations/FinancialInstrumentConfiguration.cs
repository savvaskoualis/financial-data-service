using FinancialDataService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialDataService.Infrastructure.EntityConfigurations
{
    public class FinancialInstrumentConfiguration : IEntityTypeConfiguration<FinancialInstrument>
    {
        public void Configure(EntityTypeBuilder<FinancialInstrument> builder)
        {
            builder.ToTable("FinancialInstruments");

            builder.HasKey(fi => fi.Id);

            builder.Property(fi => fi.Id)
                .ValueGeneratedOnAdd();

            builder.Property(fi => fi.Symbol)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(fi => fi.BaseAsset)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(fi => fi.QuoteAsset)
                .IsRequired()
                .HasMaxLength(10);
        }
    }
}