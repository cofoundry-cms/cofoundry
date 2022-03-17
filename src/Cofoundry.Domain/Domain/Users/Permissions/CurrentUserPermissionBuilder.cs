using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Used to extend <see cref="IPermissionSetBuilder"/> to make it easier to
/// include or exclude user permissions for the currently signed in user.
/// </summary>
public class CurrentUserPermissionBuilder : EntityPermissionBuilder
{
    public CurrentUserPermissionBuilder(
        IPermissionSetBuilder permissionSetBuilder,
        bool isIncludeOperation
        )
        : base(permissionSetBuilder, isIncludeOperation)
    {
    }

    /// <summary>
    /// All permissions, including Update, Delete and the Cofoundry user admin module permissions.
    /// </summary>
    public CurrentUserPermissionBuilder All()
    {
        return Update().Delete().AdminModule();
    }

    /// <summary>
    /// Permission to update the currently signed in user account.
    /// </summary>
    public CurrentUserPermissionBuilder Update()
    {
        Assign<CurrentUserUpdatePermission>();
        return this;
    }

    /// <summary>
    /// Permission to delete the currently signed in user account.
    /// </summary>
    public CurrentUserPermissionBuilder Delete()
    {
        Assign<CurrentUserDeletePermission>();
        return this;
    }

    /// <summary>
    /// Permission to access the current user account module in the admin panel. This only applied
    /// to the current user in the Cofoundry admnin user area and not to any custom user areas.
    /// </summary>
    public CurrentUserPermissionBuilder AdminModule()
    {
        Assign<CurrentUserAdminModulePermission>();
        return this;
    }
}
