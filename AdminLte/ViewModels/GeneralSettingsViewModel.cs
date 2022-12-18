using System.ComponentModel.DataAnnotations;

namespace AdminLte.ViewModels
{
    public class GeneralSettingsViewModel
    {
        public string Tab { get; set; }
        [Required]
        public string WebsiteName { get; set; }
        public string LogoPath { get; set; }
        public string FaviconPath { get; set; }
        public string GoogleAnalyticsTrackingCode { get; set; }
        public string WhatsappNumber { get; set; }
        public string LoginVia { get; set; }
        public string DefaultCurrency { get; set; }
        public string DefaultLanguage { get; set; }


        public IFormFile Logo { get; set; }
        public IFormFile Favicon { get; set; }

       
    }

  
}
