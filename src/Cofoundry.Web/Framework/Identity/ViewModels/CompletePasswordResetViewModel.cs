using Cofoundry.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Web.Identity
{
    public class CompletePasswordResetViewModel : ICompletePasswordResetViewModel
    {
        public CompletePasswordResetViewModel() { }

        public CompletePasswordResetViewModel(PasswordResetRequestValidationResult validationResult)
        {
            if (validationResult != null)
            {
                UserPasswordResetRequestId = validationResult.UserPasswordResetRequestId;
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
        public Guid UserPasswordResetRequestId { get; set; }

        [Required]
        public string Token { get; set; }
    }
}