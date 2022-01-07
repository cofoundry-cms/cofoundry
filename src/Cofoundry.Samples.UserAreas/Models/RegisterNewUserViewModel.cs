using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Samples.UserAreas
{
    public class RegisterNewUserViewModel
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password does not match")]
        public string ConfirmNewPassword { get; set; }
    }
}
