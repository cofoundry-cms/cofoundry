namespace Cofoundry.Web;

/// <summary>
/// Used to construct or reference the names of authorization policies
/// defined by Cofoundry.
/// </summary>
public static class AuthorizationPolicyNames
{
    /// <summary>
    /// Formats the name of a policy that restricts authorization to
    /// a specific user area.
    /// </summary>
    /// <param name="userAreaCode">The unique 3 character identifier for the user area to restrict access to.</param>
    /// <returns>Namespaced policy name in the format 'Cofoundry_UserArea_{userAreaCode}'</returns>
    public static string UserArea(string userAreaCode)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(userAreaCode);

        return $"Cofoundry_UserArea_{userAreaCode}";
    }

    /// <summary>
    /// Formats the name of a policy that restricts authorization to
    /// a specific Cofoundry role.
    /// </summary>
    /// <param name="userAreaCode">
    /// The unique 3 charcter code identifier for the user area that the role belongs to.
    /// </param>
    /// <param name="roleCode">
    /// The unique 3 character identifier for the role that the requirement 
    /// should authorize.
    /// </param>
    /// <returns>Namespaced policy name in the format 'Cofoundry_UserArea_{userAreaCode}_Role_{roleCode}'</returns>
    public static string Role(string userAreaCode, string roleCode)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(userAreaCode);
        ArgumentEmptyException.ThrowIfNullOrWhitespace(roleCode);

        return UserArea(userAreaCode) + $"_Role_{roleCode}";
    }

    /// <summary>
    /// Formats the name of a policy that restricts authorization to
    /// a specific Cofoundry permission.
    /// </summary>
    /// <param name="permission">
    /// The permission to restrict the policy to.
    /// </param>
    /// <returns>Namespaced policy name in the format 'Cofoundry_Permission_{identifier}'</returns>
    public static string Permission(IPermission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);

        var identifier = PermissionIdentifierFormatter.GetUniqueIdentifier(permission);
        return $"Cofoundry_Permission_{identifier}";
    }

    /// <summary>
    /// Formats the name of a policy that restricts authorization to
    /// a specific Cofoundry permission. This version is designed for
    /// permissions that are no scoped to a specific entity type.
    /// </summary>
    /// <param name="permissionTypeCode">
    /// The 6 character permission code to restrict the policy to. Note that this is
    /// the code for a permission that is not scoped to an entity.
    /// </param>
    /// <returns>Namespaced policy name in the format 'Cofoundry_Permission_{identifier}'</returns>
    public static string Permission(string permissionTypeCode)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(permissionTypeCode);

        if (permissionTypeCode.Length != 6)
        {
            // Ensure that this parameter isn't used for the 12 character identifier
            throw new ArgumentException($"The {nameof(permissionTypeCode)} parameter should be a 6 charcter permission type code, the specified value is {permissionTypeCode.Length} characters.", nameof(permissionTypeCode));
        }

        var identifier = PermissionIdentifierFormatter.GetUniqueIdentifier(permissionTypeCode);
        return $"Cofoundry_Permission_{identifier}";
    }

    /// <summary>
    /// Formats the name of a policy that restricts authorization to
    /// a specific Cofoundry permission.
    /// </summary>
    /// <param name="permissionTypeCode">
    /// The 6 character permission code to restrict the policy to.
    /// </param>
    /// <param name="entityDefinitionCode">
    /// The 6 character custom entity definition code to restrict the policy to. Note that this is
    /// the code for a permission that is not scoped to an entity.
    /// </param>
    /// <returns>Namespaced policy name in the format 'Cofoundry_Permission_{identifier}'</returns>
    public static string Permission(string permissionTypeCode, string entityDefinitionCode)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(permissionTypeCode);
        ArgumentEmptyException.ThrowIfNullOrWhitespace(entityDefinitionCode);

        var identifier = PermissionIdentifierFormatter.GetUniqueIdentifier(permissionTypeCode, entityDefinitionCode);
        return $"Cofoundry_Permission_{identifier}";
    }
}
