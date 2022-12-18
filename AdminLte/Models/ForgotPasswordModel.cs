using System.ComponentModel.DataAnnotations;

namespace AdminLte.Models
{
    public class ForgotPasswordModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        public bool EmailSent { get; set; }
    }
}
