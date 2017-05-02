using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Web.Identity
{
    public class CompletePasswordResetViewModel : ICompletePasswordResetViewModel
    {
        [Required]
        [Display(Name = "New password")]
        [StringLength(300, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm new password")]
        [StringLength(300, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password does not match")]
        public string ConfirmNewPassword { get; set; }

        [Required]
        public string UserPasswordResetRequestId { get; set; }

        [Required]
        public string Token { get; set; }
    }
}