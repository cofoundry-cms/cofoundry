namespace Cofoundry.Domain;

/// <summary>
/// Permission to manage the set of access rule that
/// govern who has permission to view pages in a directory and what
/// action should be taken if permission is denied.
/// </summary>
public class PageDirectoryAccessRuleManagePermission : IEntityPermission
{
    public PageDirectoryAccessRuleManagePermission()
    {
        EntityDefinition = new PageDirectoryEntityDefinition();
        PermissionType = new PermissionType("ACCRUL", "Manage Directory Access Rules", "Manage the access rules for a page directory");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
