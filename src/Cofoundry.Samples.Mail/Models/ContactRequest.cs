using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Samples.Mail
{
    /// <summary>
    /// A simple contact request model used in our
    /// contact form.
    /// </summary>
    public class ContactRequest
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Message { get; set; }
    }
}