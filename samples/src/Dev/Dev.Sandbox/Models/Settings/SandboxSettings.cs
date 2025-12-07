using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Configuration;

namespace Dev.Sandbox;

public class SandboxSiteSettings : IConfigurationSettings
{
    [Required]
    [EmailAddress]
    public string ContactRequestNotificationToAddress { get; set; } = string.Empty;
}
