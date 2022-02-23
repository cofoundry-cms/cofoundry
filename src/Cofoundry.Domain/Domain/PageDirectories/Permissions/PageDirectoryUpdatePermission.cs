namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to update a page directory, but not to update the url path.
    /// </summary>
    public class PageDirectoryUpdatePermission : IEntityPermission
    {
        public PageDirectoryUpdatePermission()
        {
            EntityDefinition = new PageDirectoryEntityDefinition();
            PermissionType = CommonPermissionTypes.Update("Page Directories");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}