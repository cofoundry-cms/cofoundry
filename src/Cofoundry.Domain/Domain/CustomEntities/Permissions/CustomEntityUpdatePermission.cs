﻿namespace Cofoundry.Domain;

/// <summary>
/// Permission to update a custom entity, but not to update a url
/// or publish.
/// </summary>
public class CustomEntityUpdatePermission : ICustomEntityPermissionTemplate
{
    /// <summary>
    /// Constructor used internally by AuthorizePermissionAttribute.
    /// </summary>
    public CustomEntityUpdatePermission()
    {
        PermissionType = CommonPermissionTypes.Update("Not Set");
        EntityDefinition = new CustomEntityDynamicEntityDefinition();
    }

    public CustomEntityUpdatePermission(ICustomEntityDefinition customEntityDefinition)
    {
        EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
        PermissionType = CommonPermissionTypes.Update(customEntityDefinition.NamePlural);
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }

    public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
    {
        var implementedPermission = new CustomEntityUpdatePermission(customEntityDefinition);
        return implementedPermission;
    }
}
