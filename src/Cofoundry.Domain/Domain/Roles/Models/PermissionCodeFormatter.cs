namespace Cofoundry.Domain;

/// <summary>
/// Helpers for formatting unique codes that represents a permissions. This is useful when
/// representing the permission as a unique string identifier.
/// </summary>
public class PermissionIdentifierFormatter
{
    /// <summary>
    /// Creates a unique code that represents this permission by combining the
    /// entity definition code and the permission code. This is useful when
    /// representing the permission as a unique string identifier.
    /// </summary>
    /// <param name="permission">Permission to get the unique code for.</param>
    /// <returns>A string identifier that uniquely identifies this permission.</returns>
    public static string GetUniqueIdentifier(IPermission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);

        if (permission is IEntityPermission customEntityPermission)
        {
            return GetUniqueIdentifier(permission.PermissionType.Code, customEntityPermission.EntityDefinition.EntityDefinitionCode);
        }
        else
        {
            return GetUniqueIdentifier(permission.PermissionType.Code);
        }
    }

    /// <summary>
    /// Creates a unique code that represents this permission by combining the
    /// entity definition code and the permission code. This is useful when
    /// representing the permission as a unique string identifier.
    /// </summary>
    /// <param name="permissionTypeCode">6 character code representing the permission type e.g. "COMRED", "COMDEL"</param>
    /// <param name="entityDefinitionCode">Optional 6 charcter entity definition code if this permission is scoped to an entity type e.g. "COFPAG", "COFIMG".</param>
    /// <returns>A string identifier that uniquely identifies this permission.</returns>
    public static string GetUniqueIdentifier(string permissionTypeCode, string entityDefinitionCode = null)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(permissionTypeCode);

        string code;

        if (!string.IsNullOrWhiteSpace(entityDefinitionCode))
        {
            code = entityDefinitionCode + permissionTypeCode;
        }
        else
        {
            code = permissionTypeCode;
        }

        return code.ToUpperInvariant();
    }
}
