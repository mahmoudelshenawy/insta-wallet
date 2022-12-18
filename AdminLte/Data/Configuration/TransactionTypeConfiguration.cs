using AdminLte.Data.Entities;
using AdminLte.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminLte.Data.Configuration
{
    public class TransactionTypeConfiguration : IEntityTypeConfiguration<TransactionTypes>
    {
        public void Configure(EntityTypeBuilder<TransactionTypes> builder)
        {
            builder.Property(p => p.Name).HasConversion(n => n.ToString() , t => Enum.Parse<TransactionTypeEnum>(t));
        }
    }
}
