using System.ComponentModel.DataAnnotations;

namespace AdminLte.Areas.User.Models
{
    public class RegisterUserModel
    {
        [Required(ErrorMessage = "please enter your first name")]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FormattedPhone { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessage = "please enter a valid pgone")]
        [Required(ErrorMessage = "please enter your phone")]
        public string Phone { get; set; }

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
        [Required(ErrorMessage = "please select a type")]
        public string Type { get; set; }

        public string CarrierCode { get; set; }
        public string DefaultCountry { get; set; }

    }
}
