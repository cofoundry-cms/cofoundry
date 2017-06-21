using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class SetupCofoundryCommandDto
    {
        [Required]
        [StringLength(50)]
        public string ApplicationName { get; set; }

        [Required]
        [StringLength(32)]
        public string UserFirstName { get; set; }

        [Required]
        [StringLength(32)]
        public string UserLastName { get; set; }

        [Required]
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Please use a valid email address")]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 8)]
        public string UserPassword { get; set; }
    }
}