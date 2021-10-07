namespace Cofoundry.Domain
{
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
}
