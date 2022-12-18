using AdminLte.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminLte.Data.Entities
{
    public class Wallet
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CurrencyId { get; set; }
        public decimal Balance { get; set; }
        public bool IsDefault { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser  User { get; set; }

        public Currency Currency { get; set; }

    }
}
