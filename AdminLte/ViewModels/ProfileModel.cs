using System.ComponentModel.DataAnnotations;

namespace AdminLte.ViewModels
{
    public class ProfileModel
    {
        [Required(ErrorMessage = "please add your username")]
        public string UserName { get; set; }

        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "please add your first name")]

        public string FirstName { get; set; }
        [Required(ErrorMessage = "please add your last name")]
        public string LastName { get; set; }

        public IFormFile ProfileImage { get; set; }

        public string ProfilePath { get; set; }
    }
}
