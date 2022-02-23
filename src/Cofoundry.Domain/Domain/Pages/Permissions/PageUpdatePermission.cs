namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to update a page, but not to update a url
    /// or publish.
    /// </summary>
    public sealed class PageUpdatePermission : IEntityPermission
    {
        public PageUpdatePermission()
        {
            EntityDefinition = new PageEntityDefinition();
            PermissionType = CommonPermissionTypes.Update("Pages");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
