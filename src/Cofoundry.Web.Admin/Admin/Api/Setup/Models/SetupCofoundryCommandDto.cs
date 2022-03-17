namespace Cofoundry.Web.Admin;

public class SetupCofoundryCommandDto
{
    [Required]
    [StringLength(50)]
    public string ApplicationName { get; set; }

    [StringLength(150)]
    public string DisplayName { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
