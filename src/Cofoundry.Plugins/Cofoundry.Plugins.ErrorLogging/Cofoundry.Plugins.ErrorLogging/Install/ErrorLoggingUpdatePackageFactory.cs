using Cofoundry.Core;
using Cofoundry.Core.AutoUpdate;

namespace Cofoundry.Plugins.ErrorLogging;

public class ErrorLoggingUpdatePackageFactory : BaseDbOnlyUpdatePackageFactory
{
    public override string ModuleIdentifier
    {
        get
        {
            return ErrorLoggingModuleInfo.ModuleIdentifier;
        }
    }

    public override IReadOnlyCollection<string> DependentModules { get; } = [CofoundryModuleInfo.ModuleIdentifier];
}
