using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SPASite.Domain;

public class RegisterMemberAndSignInCommand : ICommand
{
    [Required]
    [StringLength(150)]
    [EmailAddress(ErrorMessage = "Please use a valid email address")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [StringLength(300, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [OutputValue]
    public int OutputMemberId { get; set; }
}
