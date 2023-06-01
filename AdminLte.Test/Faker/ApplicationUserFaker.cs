using AdminLte.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AdminLte.Test.Faker
{
    internal class ApplicationUserFaker
    {
       public List<ApplicationUser> GenerateApplicationUser()
        {
            var users = new List<ApplicationUser>();
            byte[] passwordHash = new byte[] { };
            using (SHA256 alg = SHA256.Create()) {
                
              passwordHash = alg.ComputeHash(Encoding.UTF8.GetBytes("123456"));
            };
            var user = new ApplicationUser
            {
                Id = "093732b4-e94b-40b0-a0ee-8dab1192aaba",
                AddressVerified = true,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = "User@gmail.com",
                EmailConfirmed = true,
                FirstName = "Mahmoud",
                LastName = "Elshenawy",
                NormalizedEmail = "USER@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "Mahmoud",
                NormalizedUserName = "MAHMOUD",
                Status = "Active",
                Type = "User",
                PhoneNumber = "012345678",
                PasswordHash = passwordHash.ToString()
            };

            users.Add(user);
            return users;
        }
    }
}
