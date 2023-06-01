using AdminLte.Data.Entities;

namespace AdminLte.DataTableViewModels
{
    public class DepositsDataTable
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Currency { get; set; }
        public string? Uuid { get; set; }
        public string PaymentType { get; set; }
        public decimal FixedFeeAmount { get; set; }
        public decimal? PercentFeeAmount { get; set; }
        public decimal Fees { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
