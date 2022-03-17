namespace Cofoundry.Domain;

/// <summary>
/// The super administrator role is always present in the system is
/// is the highest level of access, includes all permissions and
/// cannot be changed.
/// </summary>
/// <inheritdoc/>
public sealed class SuperAdminRole : IRoleDefinition
{
    /// <summary>
    /// Constant value for the Super Administrator role code
    /// </summary>
    public const string Code = "SUP";

    [Obsolete("Renamed to 'Code' for consistency with other definitions.")]
    public const string SuperAdminRoleCode = Code;

    public string Title { get { return "Super Administrator"; } }

    public string RoleCode { get { return Code; } }

    public string UserAreaCode { get { return CofoundryAdminUserArea.Code; } }

    public void ConfigurePermissions(IPermissionSetBuilder builder)
    {
        builder.IncludeAll();
    }
}
