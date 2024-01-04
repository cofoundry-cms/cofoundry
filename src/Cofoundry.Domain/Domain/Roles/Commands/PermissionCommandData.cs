namespace Cofoundry.Domain;

public class PermissionCommandData
{
    [StringLength(6, MinimumLength = 6)]
    public string? EntityDefinitionCode { get; set; }

    [StringLength(6, MinimumLength = 6)]
    [Required]
    public string PermissionCode { get; set; } = string.Empty;
}
