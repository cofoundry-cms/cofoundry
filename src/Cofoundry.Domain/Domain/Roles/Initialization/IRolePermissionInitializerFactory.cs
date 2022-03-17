namespace Cofoundry.Domain.Internal;

/// <summary>
/// Factory for creating new <see cref="IRolePermissionInitializer"/> instances,
/// which requires some non-trivial dependency resolving to ensure the correct 
/// type is returned.
/// </summary>
public interface IRolePermissionInitializerFactory
{
    /// <summary>
    /// Creates a new <see cref="IRolePermissionInitializer"/> instance
    /// matching the requirements of the specified <paramref name="roleDefinition"/>.
    /// By default this means checking for a custom <see cref="IAnonymousRolePermissionConfiguration"/>
    /// implementation and returning it for the anonymous role.
    /// </summary>
    /// <param name="roleDefinition">
    /// The role determines the concrete type of the <see cref="IRolePermissionInitializer"/>.
    /// </param>
    IRolePermissionInitializer Create(IRoleDefinition roleDefinition);
}
