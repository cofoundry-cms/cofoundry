using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Web.Identity
{
    /// <inheritdoc/>
    public class CompleteAccountRecoveryViewModel : ICompleteAccountRecoveryViewModel
    {
        public CompleteAccountRecoveryViewModel() { }

        public CompleteAccountRecoveryViewModel(AccountRecoveryRequestValidationResult validationResult)
        {
            if (validationResult != null)
            {
                Token = validationResult.Token;
            }
        }

        [Required]
        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm new password")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Password does not match")]
        public string ConfirmNewPassword { get; set; }

        [Required]
        public string Token { get; set; }
    }
}