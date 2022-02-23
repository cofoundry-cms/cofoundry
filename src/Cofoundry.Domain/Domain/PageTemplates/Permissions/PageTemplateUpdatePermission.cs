namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to update a page template.
    /// </summary>
    public class PageTemplateUpdatePermission : IEntityPermission
    {
        public PageTemplateUpdatePermission()
        {
            EntityDefinition = new PageTemplateEntityDefinition();
            PermissionType = CommonPermissionTypes.Update("Page Templates");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}