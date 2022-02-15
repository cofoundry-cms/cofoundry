using Cofoundry.Core.AutoUpdate;
using Cofoundry.Core.Configuration;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Installation
{
    public class ValidateMigratedSettingsCommandHandler : IAsyncAlwaysRunUpdateCommandHandler<ValidateMigratedSettingsCommand>
    {
        private readonly IConfiguration _configuration;

        private Dictionary<string, string> ConfigMappings = new Dictionary<string, string>()
        {
            { "Cofoundry:Authentication:NumHoursPasswordResetLinkValid", "Cofoundry:Users:AccountRecovery:ExpireAfter" },
            { "Cofoundry:Authentication:MaxUsernameAttemptsBoundaryInMinutes", "Cofoundry:Users:Authentication:UsernameRateLimit:Window" },
            { "Cofoundry:Authentication:MaxUsernameAttempts", "Cofoundry:Users:Authentication:UsernameRateLimit:Quantity" },
            { "Cofoundry:Authentication:MaxIPAttemptsBoundaryInMinutes", "Cofoundry:Users:Authentication:IPAddressRateLimit:Window" },
            { "Cofoundry:Authentication:MaxIPAttempts", "Cofoundry:Users:Authentication:IPAddressRateLimit:Quantity" },
            { "Cofoundry:Authentication:CookieNamespace", "Cofoundry:Users:Cookie:Namespace" },
        };

        public ValidateMigratedSettingsCommandHandler(
            IConfiguration configuration
            )
        {
            _configuration = configuration;
        }

        public Task ExecuteAsync(ValidateMigratedSettingsCommand command)
        {
            var invalidSettings = ConfigMappings
                .Where(kvp => _configuration.GetValue<string>(kvp.Key) != null)
                .OrderBy(kvp => kvp.Key)
                .ToList();

            if (invalidSettings.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine("Invalid configuration detected. The following configuration settings have been moved to a new location:");
                foreach (var setting in invalidSettings)
                {
                    sb.AppendLine($"-'{setting.Key}' has moved to '{setting.Value}'");
                }
                sb.AppendLine("For up to date information on configuration settings, see the config reference docs at https://www.cofoundry.org/docs/references/common-config-settings");

                throw new InvalidConfigurationException(sb.ToString());
            }

            return Task.CompletedTask;
        }
    }
}