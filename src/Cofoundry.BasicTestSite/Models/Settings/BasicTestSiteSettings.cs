using Cofoundry.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.BasicTestSite;

public class BasicTestSiteSettings : IConfigurationSettings
{
    /// <summary>
    /// Setting Name = SimpleTestSite:ContactRequestNotificationToAddress
    /// </summary>
    [Required]
    [EmailAddress]
    public string ContactRequestNotificationToAddress { get; set; }
}
