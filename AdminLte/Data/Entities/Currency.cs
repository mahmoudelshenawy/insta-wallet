using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminLte.Data.Entities
{
    public class Currency
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Symbol { get; set; } 
        public string? Logo { get; set; }

        public bool Default { get; set; }

        public ExchangeFromEnum ExchangeFrom { get; set; } = ExchangeFromEnum.Local;
        public StatusEnum Status { get; set; } = StatusEnum.Active;

        public List<FeeLimit> FeeLimits { get; set; }
        public List<Deposit> Deposits { get; set; }
        public List<Transaction> Transactions { get; set; }
    }

    public enum StatusEnum
    {
        Active,
        Iactive
    }
    public enum ExchangeFromEnum
    {
        Local,
        Api
    }
}
           