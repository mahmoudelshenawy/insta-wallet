using AdminLte.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminLte.Data.Configuration
{
    public class CountriesConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasIndex(c => c.ShortName).IsUnique();
            builder.Property(c => c.IsDefault).HasDefaultValue("no");

            var countries = new List<Country>{
                new Country() { Id = 1, ShortName = "EG", Name = "Egypt", Iso3 = "EGY", NumberCode = "818", PhoneCode = "20" },
                new Country() { Id = 2, ShortName = "SA", Name = "Saudi Arabia", Iso3 = "SAU", NumberCode = "682", PhoneCode = "966" },
                new Country() { Id = 3, ShortName = "JO", Name = "Jordan", Iso3 = "JOR", NumberCode = "400", PhoneCode = "692" },
                new Country() { Id = 4, ShortName = "SY", Name = "Syrian Arab Republic", Iso3 = "SYR", NumberCode = "760", PhoneCode = "963" },
                new Country() { Id = 5, ShortName = "IQ", Name = "Iraq", Iso3 = "IRQ", NumberCode = "386", PhoneCode = "20" },
                new Country() { Id = 6, ShortName = "AE", Name = "United Arab Emirates", Iso3 = "ARE", NumberCode = "784", PhoneCode = "971" },
                new Country() { Id = 7, ShortName = "OM", Name = "Oman", Iso3 = "OMN", NumberCode = "512", PhoneCode = "968" },
            };

            builder.HasData(countries);
        }
    }
}
