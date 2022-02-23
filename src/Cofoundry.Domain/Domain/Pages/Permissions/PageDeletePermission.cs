namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to delete a page.
    /// </summary>
    public sealed class PageDeletePermission : IEntityPermission
    {
        public PageDeletePermission()
        {
            EntityDefinition = new PageEntityDefinition();
            PermissionType = CommonPermissionTypes.Delete("Pages");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}