namespace Cofoundry.Domain
{
    /// <summary>
    /// Permission to delete a page template.
    /// </summary>
    public class PageTemplateDeletePermission : IEntityPermission
    {
        public PageTemplateDeletePermission()
        {
            EntityDefinition = new PageTemplateEntityDefinition();
            PermissionType = CommonPermissionTypes.Delete("Page Templates");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}