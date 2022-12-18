using AdminLte.Data.Entities;
using AdminLte.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminLte.Data.Configuration
{
    public class PaymentMethodsConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.Property(p => p.Status).HasConversion(c => c.ToString(), c => Enum.Parse<PaymentMethodStatusEnum>(c));


        }
    }
}
