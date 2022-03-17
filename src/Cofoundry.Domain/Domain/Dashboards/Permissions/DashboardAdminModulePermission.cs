namespace Cofoundry.Domain;

/// <summary>
/// Permission to access the dashboards module in the admin panel.
/// </summary>
public class DashboardAdminModulePermission : IPermission
{
    public const string PermissionTypeCode = "COFDSH";

    public DashboardAdminModulePermission()
    {
        PermissionType = new PermissionType("COFDSH", "Dashboard", "View the dashboard in the admin panel");
    }

    public PermissionType PermissionType { get; private set; }
}
