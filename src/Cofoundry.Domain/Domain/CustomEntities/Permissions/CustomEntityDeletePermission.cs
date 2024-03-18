﻿namespace Cofoundry.Domain;

/// <summary>
/// Permission to delete a custom entity.
/// </summary>
public class CustomEntityDeletePermission : ICustomEntityPermissionTemplate
{
    /// <summary>
    /// Constructor used internally by AuthorizePermissionAttribute.
    /// </summary>
    public CustomEntityDeletePermission()
    {
        PermissionType = CommonPermissionTypes.Delete("Not Set");
        EntityDefinition = new CustomEntityDynamicEntityDefinition();
    }

    public CustomEntityDeletePermission(ICustomEntityDefinition customEntityDefinition)
    {
        EntityDefinition = new CustomEntityDynamicEntityDefinition(customEntityDefinition);
        PermissionType = CommonPermissionTypes.Delete(customEntityDefinition.NamePlural);
    }

    public IEntityDefinition EntityDefinition { get; private set; }
    public PermissionType PermissionType { get; private set; }

    public ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition)
    {
        var implementedPermission = new CustomEntityDeletePermission(customEntityDefinition);
        return implementedPermission;
    }
}
