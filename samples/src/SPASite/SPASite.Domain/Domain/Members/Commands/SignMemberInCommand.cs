using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SPASite.Domain;

public class SignMemberInCommand : ICommand
{
    [Required]
    [EmailAddress(ErrorMessage = "Please use a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
