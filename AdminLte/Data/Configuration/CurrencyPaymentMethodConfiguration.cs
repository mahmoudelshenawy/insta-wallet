using AdminLte.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminLte.Data.Configuration
{
    public class CurrencyPaymentMethodConfiguration : IEntityTypeConfiguration<CurrencyPaymentMethod>
    {
        public void Configure(EntityTypeBuilder<CurrencyPaymentMethod> builder)
        {
            builder.Property(p => p.ActivatedFor).HasComment("deposit, withdrawal single, both or none");
            builder.Property(p => p.MethodData).HasComment("input field\'s title and value like client_id, client_secret etc");
            builder.Property(p => p.ProcessingTime).HasDefaultValue(0);
        }
    }
}
