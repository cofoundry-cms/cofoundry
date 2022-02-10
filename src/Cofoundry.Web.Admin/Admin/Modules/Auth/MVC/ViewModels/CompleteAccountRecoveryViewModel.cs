using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Web.Admin.Internal
{
    public class CompleteAccountRecoveryViewModel
    {
        /// <summary>
        /// The value to set as the new account password. The password will go through 
        /// additional validation depending on the password policy configuration.
        /// </summary>
        [Required]
        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        /// <summary>
        /// The token used to verify the request.
        /// </summary>
        [Required]
        [Display(Name = "Confirm new password")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Password does not match")]
        public string ConfirmNewPassword { get; set; }
    }
}