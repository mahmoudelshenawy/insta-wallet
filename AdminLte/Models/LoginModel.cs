using System.ComponentModel.DataAnnotations;

namespace AdminLte.Models
{
    public class LoginModel
    {
        //[Required(ErrorMessage = "please enter your email")]
        public string Email { get; set; }

        //public string UserName { get; set; }

        [Required(ErrorMessage = "please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
