using AdminLte.Data.Enums;
using AdminLte.Rules;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.ComponentModel.DataAnnotations;

namespace AdminLte.ViewModels
{
    public class PaymentMethodViewModel
    {
        public int Id { get; set; }
        [Required, IsUnique(tableName: "PaymentMethods" , excludeSelf:"Id")]
        public string Name { get; set; }
        [Required]
        public PaymentMethodStatusEnum Status { get; set; }

        public FeeLimitForm? FeeLimit { get; set; }

    }
}
