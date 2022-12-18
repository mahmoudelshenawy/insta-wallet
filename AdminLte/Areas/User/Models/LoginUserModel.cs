using System.ComponentModel.DataAnnotations;

namespace AdminLte.Areas.User.Models
{
    public class LoginUserModel
    {
        [DataType(DataType.EmailAddress, ErrorMessage = "please enter a valid email")]
        [Required(ErrorMessage = "please enter your email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
