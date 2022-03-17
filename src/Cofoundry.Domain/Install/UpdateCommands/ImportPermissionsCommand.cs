using Cofoundry.Core.AutoUpdate;

namespace Cofoundry.Domain.Installation;

public class ImportPermissionsCommand : IVersionedUpdateCommand
{
    public string Description
    {
        get { return GetType().Name; }
    }

    public int Version
    {
        get { return 1; }
    }
}
