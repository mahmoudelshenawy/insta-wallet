using System.ComponentModel.DataAnnotations;

namespace AdminLte.Models
{
    public class ResetPasswordModel
    {
        public string userId { get; set; }
        public string Token { get; set; }
        [Required]
        [DataType(DataType.Password), MinLength(6)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password), MinLength(6)]
        [Compare("NewPassword" , ErrorMessage = "your password does not match")]
        public string ConfirmPassword { get; set; }

        public bool IsReset { get; set; }
    }
}
