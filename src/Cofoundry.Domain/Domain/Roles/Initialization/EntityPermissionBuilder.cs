namespace Cofoundry.Domain.Extendable;

/// <summary>
/// A base class for to help with including or excluding sets
/// of permissions for a specific entity type.
/// </summary>
public abstract class EntityPermissionBuilder
{
    private readonly IPermissionSetBuilder _permissionSetBuilder;
    private readonly bool _isIncludeOperation;

    public EntityPermissionBuilder(
        IPermissionSetBuilder permissionSetBuilder,
        bool isIncludeOperation
        )
    {
        _permissionSetBuilder = permissionSetBuilder;
        _isIncludeOperation = isIncludeOperation;
    }

    /// <summary>
    /// Assigns the specified permission to the builder result
    /// using either an include or exclude operation depending on the
    /// configuration of this instance.
    /// </summary>
    /// <typeparam name="IPermission">Permission type to assign.</typeparam>
    protected void Assign<TPermission>()
        where TPermission : IPermission
    {
        if (_isIncludeOperation)
        {
            _permissionSetBuilder.Include<TPermission>();
        }
        else
        {
            _permissionSetBuilder.Exclude<TPermission>();
        }
    }

    /// <summary>
    /// Assigns the specified permission to the builder result
    /// using either an include or exclude operation depending on the
    /// configuration of this instance.
    /// </summary>
    /// <param name="permission">Permission instance to assign.</param>
    protected void Assign<TPermission>(TPermission permission)
        where TPermission : IPermission
    {
        if (_isIncludeOperation)
        {
            _permissionSetBuilder.Include(new IPermission[] { permission });
        }
        else
        {
            _permissionSetBuilder.Exclude(new IPermission[] { permission });
        }
    }
}
