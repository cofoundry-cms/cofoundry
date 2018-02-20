using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Web.Identity
{
    public class LoginViewModel : ILoginViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        [Display(Name = "Email")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please provide your password")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}