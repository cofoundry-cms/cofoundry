namespace Cofoundry.Web.Admin.Internal;

public class ChangePasswordViewModel
{
    [DataType(DataType.EmailAddress)]
    [Required]
    [EmailAddress(ErrorMessage = "Please use a valid email address")]
    [Display(Name = "Email")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Current password")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "New password")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Confirm new password")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Password does not match")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
