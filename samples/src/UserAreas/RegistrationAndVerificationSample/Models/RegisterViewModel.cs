using System.ComponentModel.DataAnnotations;

namespace RegistrationAndVerificationSample;

public class RegisterViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Please use a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Confirm password")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Password does not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
