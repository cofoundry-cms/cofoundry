using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Web.Admin.Internal
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Username { get; set; }
    }
}