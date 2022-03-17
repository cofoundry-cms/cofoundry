namespace Cofoundry.Domain.Internal;

/// <summary>
/// Factory for creating new <see cref="IPermissionSetBuilder"/> instances,
/// which have some non-trivial dependencies to resolve during construction.
/// </summary>
public interface IPermissionSetBuilderFactory
{
    /// <summary>
    /// Creates a new <see cref="IPermissionSetBuilder"/> instance.
    /// </summary>
    /// <param name="permissionsToFilter">
    /// The set of permissions which the builder should apply filtering
    /// to.
    /// </param>
    IPermissionSetBuilder Create(IEnumerable<IPermission> permissionsToFilter);
}
