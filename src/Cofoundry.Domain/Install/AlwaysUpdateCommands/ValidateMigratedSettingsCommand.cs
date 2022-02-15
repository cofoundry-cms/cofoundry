using Cofoundry.Core.AutoUpdate;

namespace Cofoundry.Domain.Installation
{
    public class ValidateMigratedSettingsCommand : IAlwaysRunUpdateCommand
    {
        public string Description
        {
            get
            {
                return "Validates that any migrated settings are not references in configuration";
            }
        }
    }
}