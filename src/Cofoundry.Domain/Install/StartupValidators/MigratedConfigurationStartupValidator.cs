using System.Text;
using Cofoundry.Core.AutoUpdate.Internal;
using Cofoundry.Core.Configuration;
using Microsoft.Extensions.Configuration;

namespace Cofoundry.Domain.Install;

/// <summary>
/// Validates that any migrated settings are not referenced in configuration
/// </summary>
public class MigratedConfigurationStartupValidator : IStartupValidator
{
    private readonly IConfiguration _configuration;

    private readonly Dictionary<string, string> _configMappings = new()
    {
        { "Cofoundry:Authentication:NumHoursPasswordResetLinkValid", "Cofoundry:Users:AccountRecovery:ExpireAfter" },
        { "Cofoundry:Authentication:MaxUsernameAttemptsBoundaryInMinutes", "Cofoundry:Users:Authentication:UsernameRateLimit:Window" },
        { "Cofoundry:Authentication:MaxUsernameAttempts", "Cofoundry:Users:Authentication:UsernameRateLimit:Quantity" },
        { "Cofoundry:Authentication:MaxIPAttemptsBoundaryInMinutes", "Cofoundry:Users:Authentication:IPAddressRateLimit:Window" },
        { "Cofoundry:Authentication:MaxIPAttempts", "Cofoundry:Users:Authentication:IPAddressRateLimit:Quantity" },
        { "Cofoundry:Authentication:CookieNamespace", "Cofoundry:Users:Cookie:Namespace" },
    };

    public MigratedConfigurationStartupValidator(
        IConfiguration configuration
        )
    {
        _configuration = configuration;
    }

    public void Validate()
    {
        var invalidSettings = _configMappings
            .Where(kvp => _configuration.GetValue<string>(kvp.Key) != null)
            .OrderBy(kvp => kvp.Key)
            .ToArray();

        if (invalidSettings.Length != 0)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Invalid configuration detected. The following configuration settings have been moved to a new location:");
            foreach (var setting in invalidSettings)
            {
                sb.AppendLine(CultureInfo.CurrentCulture, $"-'{setting.Key}' has moved to '{setting.Value}'");
            }
            sb.AppendLine("For up to date information on configuration settings, see the config reference docs at https://www.cofoundry.org/docs/references/common-config-settings");

            throw new InvalidConfigurationException(sb.ToString());
        }
    }
}
