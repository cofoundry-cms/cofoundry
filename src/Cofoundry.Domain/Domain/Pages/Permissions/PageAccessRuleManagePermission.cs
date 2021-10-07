namespace Cofoundry.Domain
{
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
}
