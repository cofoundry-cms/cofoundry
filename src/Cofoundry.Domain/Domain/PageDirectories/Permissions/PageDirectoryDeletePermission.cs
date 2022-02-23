namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to delete a page directory.
    /// </summary>
    public class PageDirectoryDeletePermission : IEntityPermission
    {
        public PageDirectoryDeletePermission()
        {
            EntityDefinition = new PageDirectoryEntityDefinition();
            PermissionType = CommonPermissionTypes.Delete("Page Directories");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}