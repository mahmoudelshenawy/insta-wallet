using AdminLte.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminLte.Data.Configuration
{
    public class CurrenciesConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.Property(p => p.Status).HasConversion(p => p.ToString(), p => Enum.Parse<StatusEnum>(p));

            builder.Property(p => p.ExchangeFrom).HasConversion(p => p.ToString(), p => Enum.Parse<ExchangeFromEnum>(p));

            builder.Property(p => p.Type).HasDefaultValue("fiat");
            builder.Property(p => p.ExchangeFrom).HasDefaultValue(ExchangeFromEnum.Local);
            builder.Property(p => p.Status).HasDefaultValue(StatusEnum.Active);

            //optionB

           // builder.Property(e => e.Status).HasConversion<string>();
        }
    }
}
