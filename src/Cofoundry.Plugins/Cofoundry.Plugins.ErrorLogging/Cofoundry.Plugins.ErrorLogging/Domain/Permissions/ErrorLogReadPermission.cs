namespace Cofoundry.Plugins.ErrorLogging.Domain;

public class ErrorLogReadPermission : IPermission
{
    public ErrorLogReadPermission()
    {
        PermissionType = new PermissionType("ERRLOG", "Error logs", "View error logs in the admin panel");
    }

    public PermissionType PermissionType { get; private set; }
}
