using Cofoundry.Core.AutoUpdate;

namespace Cofoundry.Domain.Installation;

/// <summary>
/// Runs the RegisterEntityDefinitionsCommand at startup, adding new entity
/// definitions defined in code to the system and initializing permissions.
/// </summary>
public class RegisterPermissionsAndRolesUpdateCommand : IAlwaysRunUpdateCommand
{
    public string Description
    {
        get
        {
            return "Register new roles and permissions defined in code";
        }
    }
}
