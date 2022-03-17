namespace Cofoundry.Domain;

/// <summary>
/// Used to configure a set of permissions with a fluent style API. When <see cref="Build"/>
/// is called the permission set is compiled into a distinct collection.
/// </summary>
public interface IPermissionSetBuilder
{
    /// <summary>
    /// A collection containing the base set of permissions to filter down
    /// to a permitted set. When creating a role this collection will contain
    /// the full set of permissions in the system, however when installing 
    /// newly-added permissions to an existing role it will only contain the 
    /// new permissions.
    /// </summary>
    IEnumerable<IPermission> AvailablePermissions { get; }

    /// <summary>
    /// Adds the specified <paramref name="permissions"/> to the builder
    /// configuration. Duplicate permissions are discarded.
    /// </summary>
    /// <param name="permissions">
    /// The collection of permissions to include in the configuration.
    /// </param>
    IPermissionSetBuilder Include(IEnumerable<IPermission> permissions);

    /// <summary>
    /// Adds a set of permissions to the builder configuration, filtered 
    /// from the <see cref="AvailablePermissions"/> using the specified
    /// <paramref name="filter"/> function.
    /// </summary>
    /// <param name="filter">
    /// A filter function to run on the <see cref="AvailablePermissions"/>
    /// collection before adding the result to the configuration.
    /// </param>
    IPermissionSetBuilder Include(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> filter);

    /// <summary>
    /// Removes the specified <paramref name="permissions"/> from the builder
    /// configuration.
    /// </summary>
    /// <param name="permissions">
    /// The collection of permissions to remove from the configuration. Any
    /// permissions not in the configuration are ignored.
    /// </param>
    IPermissionSetBuilder Exclude(IEnumerable<IPermission> permissions);

    /// <summary>
    /// Removes a set of permissions to the builder configuration, filtered 
    /// from the <see cref="AvailablePermissions"/> using the specified
    /// <paramref name="filter"/> function.
    /// </summary>
    /// <param name="filter">
    /// A filter function to run on the <see cref="AvailablePermissions"/>
    /// collection before removing the result from the configuration.
    /// </param>
    IPermissionSetBuilder Exclude(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> filter);

    /// <summary>
    /// Compiles the configuration into a distinct set of permissions.
    /// </summary>
    ICollection<IPermission> Build();

    /// <summary>
    /// Copies a permission set from an existing role configuration.
    /// </summary>
    /// <typeparam name="TRoleDefinition">The <see cref="IRoleDefinition"/> to copy configuration from.</typeparam>
    /// <param name="additionalFilter">An additional filter to run before adding the role permissions to the builder.</param>
    IPermissionSetBuilder ApplyRoleConfiguration<TRoleDefinition>(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
       where TRoleDefinition : IRoleDefinition;

    /// <summary>
    /// Adds the specified permission type to the builder configuration. Permissions
    /// are selected based on a type check, so a base type or interface can be used
    /// to add multiple types. Duplicate permissions are discarded.
    /// </summary>
    /// <typeparam name="TPermission">
    /// A concrete, interface or base type to filter permissions on.
    /// </typeparam>
    IPermissionSetBuilder Include<TPermission>() where TPermission : IPermission;

    /// <summary>
    /// Adds all permissions for the specified entity type to the builder configuration.
    /// Duplicate permissions are discarded.
    /// </summary>
    /// <typeparam name="TEntityDefinition">The <see cref="IEntityDefinition"/> to filter permissions by.</typeparam>
    /// <param name="additionalFilter">An additional filter to run before adding the permissions to the builder.</param>
    IPermissionSetBuilder IncludeEntity<TEntityDefinition>(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
       where TEntityDefinition : IEntityDefinition;

    /// <summary>
    /// Adds all permissions for the specified <paramref name="permissionTypeCode"/> to the 
    /// builder configuration. For common permissions types it's easier just to call the 
    /// type specified method e.g. <see cref="IncludeAllRead"/>. Duplicate permissions are 
    /// discarded.
    /// </summary>
    /// <param name="permissionTypeCode">The unique 6 character code of the permission type to add.</param>
    /// <param name="additionalFilter">An additional filter to run before adding the permissions to the builder.</param>
    IPermissionSetBuilder IncludeAllWithPermissionType(string permissionTypeCode, Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);

    /// <summary>
    /// Adds permissions that use the Read common permission type to the builder
    /// configuration. Duplicate permissions are discarded.
    /// </summary>
    /// <param name="additionalFilter">An additional filter to run before adding the permissions to the builder.</param>
    IPermissionSetBuilder IncludeAllRead(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);

    /// <summary>
    /// Adds permissions that use the Update common permission type to the builder
    /// configuration. This does not include special permissions such as <see cref="PageUpdateUrlPermission"/>. 
    /// Duplicate permissions are discarded.
    /// </summary>
    /// <param name="additionalFilter">An additional filter to run before adding the permissions to the builder.</param>
    IPermissionSetBuilder IncludeAllUpdate(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);

    /// <summary>
    /// Adds permissions that use the Create common permission type to the builder
    /// configuration. Duplicate permissions are discarded.
    /// </summary>
    /// <param name="additionalFilter">An additional filter to run before adding the permissions to the builder.</param>
    IPermissionSetBuilder IncludeAllCreate(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);

    /// <summary>
    /// Adds permissions that use the Delete common permission type to the builder
    /// configuration. Duplicate permissions are discarded.
    /// </summary>
    /// <param name="additionalFilter">An additional filter to run before adding the permissions to the builder.</param>
    IPermissionSetBuilder IncludeAllDelete(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);

    /// <summary>
    /// Adds permissions that use one of the common permission types associated with writing 
    /// data to the builder configuration. This can include the "Create", "Update" and "Delete"
    /// permissions as well as the more generic "Write" permission. This does not include special 
    /// permissions such as <see cref="PageUpdateUrlPermission"/>. Duplicate permissions are discarded.
    /// </summary>
    /// <param name="additionalFilter">An additional filter to run before adding the permissions to the builder.</param>
    IPermissionSetBuilder IncludeAllWrite(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);

    /// <summary>
    /// Adds all permissions that grant access to admin panel sections to the builder 
    /// configuration. Specifically this includes permissions that use the admin module common permission type 
    /// code as well as the dashboard permission type code. Duplicate permissions are discarded.
    /// </summary>
    /// <param name="additionalFilter">An additional filter to run before adding the permissions to the builder.</param>
    IPermissionSetBuilder IncludeAllAdminModule(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);

    /// <summary>
    /// <para>
    /// Uses the default permission configuration for the <see cref="AnonymousRole"/> to assign 
    /// permissions to the builder configuration. The anonymous role by default can read any 
    /// entity except for users. This is because user read permission means 'all users' not just 
    /// 'current user' and is associated with user management.
    /// </para>
    /// <para>
    /// This method differs from <see cref="IPermissionSetBuilderExtensions.ApplyAnonymousRoleConfiguration"/> 
    /// because it ignores any custom <see cref="IAnonymousRolePermissionConfiguration"/> implementation and
    /// instead uses the Cofoundry default rules. Duplicate permissions are discarded.
    /// </para>
    /// </summary>
    /// <param name="additionalFilter">An additional filter to run before adding the permissions to the builder.</param>
    IPermissionSetBuilder IncludeAnonymousRoleDefaults(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);

    /// <summary>
    /// Removes the specified permission type from the builder configuration. Permissions
    /// are selected based on a type check, so a base type or interface can be used
    /// to add multiple types.
    /// </summary>
    /// <typeparam name="TPermission">
    /// A concrete, interface or base type to filter permissions on.
    /// </typeparam>
    IPermissionSetBuilder Exclude<TPermission>();

    /// <summary>
    /// Removes all permissions for the specified entity type from the builder configuration.
    /// </summary>
    /// <typeparam name="TEntityDefinition">The <see cref="IEntityDefinition"/> to filter permissions by.</typeparam>
    /// <param name="additionalFilter">An additional filter to run before removing the permissions from the builder.</param>
    IPermissionSetBuilder ExcludeEntity<TEntityDefinition>(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
        where TEntityDefinition : IEntityDefinition;

    /// <summary>
    /// Removes all permissions with the specified <paramref name="permissionTypeCode"/> from the 
    /// builder configuration. For common permissions types it's easier just to call the 
    /// type specified method e.g. <see cref="ExcludeAllDelete"/>.
    /// </summary>
    /// <param name="permissionTypeCode">The unique 6 character code of the permission type to exclude.</param>
    /// <param name="additionalFilter">An additional filter to run before removing the permissions from the builder.</param>
    IPermissionSetBuilder ExcludeAllWithPermissionType(string permissionTypeCode, Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);

    /// <summary>
    /// Removes permissions that use the Create common permission type from the builder
    /// configuration. 
    /// </summary>
    /// <param name="additionalFilter">An additional filter to run before removing the permissions from the builder.</param>
    IPermissionSetBuilder ExcludeAllCreate(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);

    /// <summary>
    /// Removes permissions that use the Update common permission type from the builder
    /// configuration. This does not remove special permissions such as <see cref="PageUpdateUrlPermission"/>. 
    /// </summary>
    /// <param name="additionalFilter">An additional filter to run before removing the permissions from the builder.</param>
    IPermissionSetBuilder ExcludeAllUpdate(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);

    /// <summary>
    /// Removes permissions that use the Delete common permission type from the builder
    /// configuration.
    /// </summary>
    /// <param name="additionalFilter">An additional filter to run before removing the permissions from the builder.</param>
    IPermissionSetBuilder ExcludeAllDelete(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);

    /// <summary>
    /// Removes permissions that use one of the common permission types associated with writing 
    /// data from the builder configuration. This can include the "Create", "Update" and "Delete"
    /// permissions as well as the more generic "Write" permission. This does not include special 
    /// permissions such as <see cref="PageUpdateUrlPermission"/>.
    /// </summary>
    /// <param name="additionalFilter">An additional filter to run before removing the permissions from the builder.</param>
    IPermissionSetBuilder ExcludeAllWrite(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);

    /// <summary>
    /// Removes all permissions that grant access to sections of the admin panel from the builder 
    /// configuration. Specifically this includes permissions that use the admin module common permission type 
    /// code as well as the dashboard permission type code.
    /// </summary>
    /// <param name="additionalFilter">An additional filter to run before removing the permissions from the builder.</param>
    IPermissionSetBuilder ExcludeAdminModule(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null);
}
