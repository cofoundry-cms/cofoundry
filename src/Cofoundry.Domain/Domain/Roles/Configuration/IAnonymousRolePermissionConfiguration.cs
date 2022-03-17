namespace Cofoundry.Domain;

/// <summary>
/// Implementing <see cref="IAnonymousRolePermissionConfiguration"/> allows you to 
/// redefine the permissions granted to the anonymouse role. This is invoked when the 
/// anonymous role is first created, and is also used to update it when new permissions are
/// added to the system.
/// </summary>
public interface IAnonymousRolePermissionConfiguration
{
    /// <summary>
    /// Configures the permissions that that should be added to the anonymous role when
    /// it is first created. This is also invoked when new permissions are added
    /// to the system to determine whether it should be automatically added to the 
    /// role, based on the rules defined in the <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">
    /// Use the builder to configure the rules that determine whether a permission
    /// should be added to the system. A good base to start from is calling
    /// <see cref="IPermissionSetBuilder.IncludeAnonymousRoleDefaults"/> which
    /// will include the baseline permissions from the default anonymous role
    /// configuration.
    /// </param>
    void ConfigurePermissions(IPermissionSetBuilder builder);
}
