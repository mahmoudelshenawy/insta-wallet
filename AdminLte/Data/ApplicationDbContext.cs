using AdminLte.Data.Entities;
using AdminLte.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminLte.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options):base(options)
        {

        }

        public DbSet<Settings> Settings { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<TransactionTypes> TransactionTypes { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CurrencyPaymentMethod> CurrencyPaymentMethods { get; set; }
        public DbSet<FeeLimit> FeesLimits { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Bank> Banks { get; set; }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Dummy> Dummies { get; set; }
        public DbSet<Deposit> Deposits { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            //     builder.Entity<IdentityRole>()
            //         .HasData(new IdentityRole
            //         {
            //             Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //             Name = "Admin",
            //             NormalizedName = "ADMIN".ToUpper()
            //         }
            //         );

            //     var hasher = new PasswordHasher<ApplicationUser>();


            //     //Seeding the User to AspNetUsers table
            //     builder.Entity<ApplicationUser>().HasData(
            //         new ApplicationUser
            //         {
            //             Id = "8e445865-a24d-4543-a6c6-9443d048cdb9", // primary key
            //             FirstName = "Admin",
            //             UserName = "Admin",
            //             LastName = "",
            //             NormalizedUserName = "ADMIN",
            //             Email = "admin@gmail.com",
            //             PasswordHash = hasher.HashPassword(null, "123456")
            //         }
            //     );

            //     builder.Entity<IdentityUserRole<string>>().HasData(
            //    new IdentityUserRole<string>
            //    {
            //        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
            //        UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
            //    }
            //);
        }
    }
}
