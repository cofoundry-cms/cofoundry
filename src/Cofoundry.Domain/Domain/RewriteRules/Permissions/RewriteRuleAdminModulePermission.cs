namespace Cofoundry.Domain;

/// <summary>
/// Permission to access the rewrite rules module in the admin panel.
/// </summary>
public class RewriteRuleAdminModulePermission : IEntityPermission
{
    public RewriteRuleAdminModulePermission()
    {
        EntityDefinition = new RewriteRuleEntityDefinition();
        PermissionType = CommonPermissionTypes.AdminModule("Rewrite Rules");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
