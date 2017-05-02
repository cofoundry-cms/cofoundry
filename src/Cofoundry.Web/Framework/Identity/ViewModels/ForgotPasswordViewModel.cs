using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Web.Identity
{
    public class ForgotPasswordViewModel : IForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Username { get; set; }
    }
}