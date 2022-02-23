namespace Cofoundry.Domain
{
    /// <summary>
    /// Read access to page templates. Read access is required in order
    /// to include any other page template permissions.
    /// </summary>
    public class PageTemplateReadPermission : IEntityPermission
    {
        public PageTemplateReadPermission()
        {
            EntityDefinition = new PageTemplateEntityDefinition();
            PermissionType = CommonPermissionTypes.Read("Page Templates");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}