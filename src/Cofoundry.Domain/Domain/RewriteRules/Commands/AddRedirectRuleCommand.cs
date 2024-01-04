using Newtonsoft.Json;

namespace Cofoundry.Domain;

public class AddRedirectRuleCommand : ICommand, ILoggableCommand
{
    [StringLength(2000)]
    [Required]
    public string WriteFrom { get; set; } = string.Empty;

    [StringLength(2000)]
    [Required]
    public string WriteTo { get; set; } = string.Empty;

    [OutputValue]
    public int OutputRedirectRuleId { get; set; }
}