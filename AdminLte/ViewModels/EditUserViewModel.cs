using System.ComponentModel.DataAnnotations;

namespace AdminLte.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "please enter user first name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "please enter user last name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "please enter username")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "please enter a valid email")]
        [Required(ErrorMessage = "please enter user email")]
        public string Email { get; set; }
    }
}
