using Microsoft.Extensions.Primitives;

namespace AdminLte.Areas.User.Models
{
    public class SuccessModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Dictionary<string , string> DataSet { get; set; } // paymentmethod + currency
        public Dictionary<string , decimal> DataSetNumbers { get; set; } //totalFees + totalAmount + percentFee + FixedFee
    }
}
