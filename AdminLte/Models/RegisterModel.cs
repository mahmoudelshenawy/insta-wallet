using System.ComponentModel.DataAnnotations;

namespace AdminLte.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "please enter your first name")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "please enter a valid email")]
        [Required(ErrorMessage = "please enter your email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "please enter your password")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "the password does not match")]
        public string Password { get; set; }
        [Required(ErrorMessage = "please confirm your password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
