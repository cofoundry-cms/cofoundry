namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to create new page directories.
    /// </summary>
    public class PageDirectoryCreatePermission : IEntityPermission
    {
        public PageDirectoryCreatePermission()
        {
            EntityDefinition = new PageDirectoryEntityDefinition();
            PermissionType = CommonPermissionTypes.Create("Page Directories");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}