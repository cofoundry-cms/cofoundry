namespace Cofoundry.Domain;

/// <summary>
/// Permission to manage the set of access rule that
/// govern who has permission to view a page and what
/// action should be taken if permission is denied.
/// </summary>
public class PageAccessRuleManagePermission : IEntityPermission
{
    public PageAccessRuleManagePermission()
    {
        EntityDefinition = new PageEntityDefinition();
        PermissionType = new PermissionType("ACCRUL", "Manage Page Access Rules", "Manage the access rules for a page");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
