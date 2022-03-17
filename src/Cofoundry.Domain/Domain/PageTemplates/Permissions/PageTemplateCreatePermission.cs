namespace Cofoundry.Domain;

/// <summary>
/// Permission to create new page templates.
/// </summary>
public class PageTemplateCreatePermission : IEntityPermission
{
    public PageTemplateCreatePermission()
    {
        EntityDefinition = new PageTemplateEntityDefinition();
        PermissionType = CommonPermissionTypes.Create("Page Templates");
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }
}
