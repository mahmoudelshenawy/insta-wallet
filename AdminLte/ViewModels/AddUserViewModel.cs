using System.ComponentModel.DataAnnotations;

namespace AdminLte.ViewModels
{
    public class AddUserViewModel
    {
        [Required(ErrorMessage = "please enter user first name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "please enter user last name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "please enter username")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "please enter a valid email")]
        [Required(ErrorMessage = "please enter user email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "please enter valid password")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "the password does not match")]
        public string Password { get; set; }
        [Required(ErrorMessage = "please confirm user password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public List<RoleViewModel> Roles { get; set; }

    }
}
