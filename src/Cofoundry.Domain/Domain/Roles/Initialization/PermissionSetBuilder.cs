using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class PermissionSetBuilder : IExtendablePermissionSetBuilder
{
    private PermissionEqualityComparer _permissionEqualityComparer = new PermissionEqualityComparer();
    private IEnumerable<IPermission> _permissions = Array.Empty<IPermission>();
    private CircularDependencyGuard _circularDependencyGuard = null;

    private readonly IRolePermissionInitializerFactory _rolePermissionInitializerFactory;
    private readonly IEntityDefinitionRepository _entityDefinitionRepository;
    private readonly IRoleDefinitionRepository _roleDefinitionRepository;

    public PermissionSetBuilder(
        IEnumerable<IPermission> permissionsToFilter,
        IServiceProvider serviceProvider,
        IRolePermissionInitializerFactory rolePermissionInitializerFactory,
        IEntityDefinitionRepository entityDefinitionRepository,
        IRoleDefinitionRepository roleDefinitionRepository
        )
    {
        AvailablePermissions = permissionsToFilter;
        ServiceProvider = serviceProvider;
        _rolePermissionInitializerFactory = rolePermissionInitializerFactory;
        _entityDefinitionRepository = entityDefinitionRepository;
        _roleDefinitionRepository = roleDefinitionRepository;
    }

    private PermissionSetBuilder(
        CircularDependencyGuard circularDependencyGuard,
        IEnumerable<IPermission> allPermissions,
        IServiceProvider serviceProvider,
        IRolePermissionInitializerFactory rolePermissionInitializerFactory,
        IEntityDefinitionRepository entityDefinitionRepository,
        IRoleDefinitionRepository roleDefinitionRepository
        )
    {
        _circularDependencyGuard = circularDependencyGuard;
        AvailablePermissions = allPermissions;
        ServiceProvider = serviceProvider;
        _rolePermissionInitializerFactory = rolePermissionInitializerFactory;
        _entityDefinitionRepository = entityDefinitionRepository;
        _roleDefinitionRepository = roleDefinitionRepository;
    }

    public IEnumerable<IPermission> AvailablePermissions { get; }

    public IServiceProvider ServiceProvider { get; }

    public ICollection<IPermission> Build()
    {
        return _permissions.ToArray();
    }

    public virtual IPermissionSetBuilder Include(IEnumerable<IPermission> permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);

        return Include(p => p.Intersect(permissions, _permissionEqualityComparer));
    }

    public virtual IPermissionSetBuilder Include(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> permissionFilter)
    {
        ArgumentNullException.ThrowIfNull(permissionFilter);

        var filteredPermissions = permissionFilter(AvailablePermissions);
        _permissions = _permissions.Union(filteredPermissions, _permissionEqualityComparer);

        return this;
    }

    private IPermissionSetBuilder Include(
        Func<IEnumerable<IPermission>, IEnumerable<IPermission>> permissionFilter,
        Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter
        )
    {
        ArgumentNullException.ThrowIfNull(permissionFilter);

        var filtered = permissionFilter(AvailablePermissions);

        if (additionalFilter != null)
        {
            filtered = additionalFilter(filtered);
        }

        return Include(filtered);
    }

    public virtual IPermissionSetBuilder Exclude(IEnumerable<IPermission> permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);

        return Exclude(p => p.Intersect(permissions, _permissionEqualityComparer));
    }

    public virtual IPermissionSetBuilder Exclude(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> permissionFilter)
    {
        ArgumentNullException.ThrowIfNull(permissionFilter);

        var permissionsToExclude = permissionFilter(AvailablePermissions);
        _permissions = _permissions.Except(permissionsToExclude, _permissionEqualityComparer);

        return this;
    }

    private IPermissionSetBuilder Exclude(
        Func<IEnumerable<IPermission>, IEnumerable<IPermission>> permissionFilter,
        Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter
        )
    {
        ArgumentNullException.ThrowIfNull(permissionFilter);

        var permissionsToExclude = permissionFilter(AvailablePermissions);

        if (additionalFilter != null)
        {
            permissionsToExclude = additionalFilter(permissionsToExclude);
        }

        return Exclude(permissionsToExclude);
    }

    /// <summary>
    /// Filters a collection of permissions to only include permissions for a specific entity type.
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <param name="entityDefinitionCode">The definition code of the entity to filter on e.g. PageEntityDefinition.DefinitionCode</param>
    /// <returns>Filtered collection cast to IEnumerable{IEntityPermission}</returns>
    public virtual IPermissionSetBuilder ApplyRoleConfiguration<TRoleDefinition>(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
        where TRoleDefinition : IRoleDefinition
    {
        var roleDefinition = _roleDefinitionRepository.GetRequired<TRoleDefinition>();
        var guard = _circularDependencyGuard;

        if (guard != null)
        {
            guard.AddRole<TRoleDefinition>();
        }
        else
        {
            guard = new CircularDependencyGuard(roleDefinition);
        }

        var builder = new PermissionSetBuilder(
            guard,
            AvailablePermissions,
            ServiceProvider,
            _rolePermissionInitializerFactory,
            _entityDefinitionRepository,
            _roleDefinitionRepository
            );

        var permissionInitializer = _rolePermissionInitializerFactory.Create(roleDefinition);
        permissionInitializer.Initialize(builder);
        var rolePermissions = builder.Build();

        return Include(permissions => rolePermissions, additionalFilter);
    }

    /// <summary>
    /// Filters a collection of permissions to only include permissions for a specific entity type.
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <param name="entityDefinitionCode">The definition code of the entity to filter on e.g. PageEntityDefinition.DefinitionCode</param>
    /// <returns>Filtered collection cast to IEnumerable{IEntityPermission}</returns>
    public virtual IPermissionSetBuilder IncludeEntity<TEntity>(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
        where TEntity : IEntityDefinition
    {
        var entityDefinition = _entityDefinitionRepository.GetRequired<TEntity>();
        return Include(permissions => permissions.FilterToEntityPermissions(entityDefinition.EntityDefinitionCode), additionalFilter);
    }

    /// <summary>
    /// Filters a collection of permissions to only include permissions for a specific permission type
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <param name="permissionCode">The code of the permission type to filter on</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder IncludeAllWithPermissionType(string permissionCode, Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return Include(permissions => permissions.Where(p => p.PermissionType.Code == permissionCode), additionalFilter);
    }

    /// <summary>
    /// Filters a collection of permissions to only include permissions that are or inherit from a specific permission type
    /// </summary>
    /// <typeparam name="TPermission">The type of permission to filter on.</typeparam>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder Include<TPermission>()
        where TPermission : IPermission
    {
        return Include(permissions => permissions.Where(p => p is TPermission));
    }

    /// <summary>
    /// Filters a collection of permissions to only include permissions that use the Read common permission type
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder IncludeAllRead(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return IncludeAllWithPermissionType(CommonPermissionTypes.ReadPermissionCode, additionalFilter);
    }

    /// <summary>
    /// Filters a collection of permissions to only include permissions that use the Update common permission type
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder IncludeAllUpdate(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return IncludeAllWithPermissionType(CommonPermissionTypes.UpdatePermissionCode, additionalFilter);
    }

    /// <summary>
    /// Filters a collection of permissions to only include permissions that use the Create common permission type
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder IncludeAllCreate(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return IncludeAllWithPermissionType(CommonPermissionTypes.CreatePermissionCode, additionalFilter);
    }

    /// <summary>
    /// Filters a collection of permissions to only include permissions that use the Delete common permission type
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder IncludeAllDelete(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return IncludeAllWithPermissionType(CommonPermissionTypes.DeletePermissionCode, additionalFilter);
    }

    /// <summary>
    /// Filters a collection of permissions to only include permissions that use common 
    /// permission types associated with writing data which can include the "Create", "Update" and "Delete"
    /// permissions as well as the more generic "Write" permission.
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder IncludeAllWrite(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return Include(permissions => permissions.FilterToWritePermissions(), additionalFilter);
    }

    /// <summary>
    /// Filters a collection of permissions to only include permissions that permit
    /// access to sections in the admin panel. Specifically permissions that use the
    /// admin module common permission type code and the dashboard permission type code.
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder IncludeAllAdminModule(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return Include(permissions => permissions.FilterToAdminModulePermissions(), additionalFilter);
    }

    /// <summary>
    /// The anonymous role by default can read any entity except for users.
    /// This is because user read permission means 'all users' not just 'current user'
    /// and is associated with user management.
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder IncludeAnonymousRoleDefaults(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return Include(permissions => permissions.FilterToAnonymousRoleDefaults(), additionalFilter);
    }

    /// <summary>
    /// Removes the specified permission from the collection.
    /// </summary>
    /// <typeparam name="TPermission">IPermission type to remove</typeparam>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder Exclude<TPermission>()
    {
        return Exclude(permissions => permissions.Where(p => p is TPermission));
    }

    /// <summary>
    /// Removes permissions with the specified permission code from the collection.
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <param name="permissionTypeCode">Code of the permission type to exclude from the collection</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder ExcludeAllWithPermissionType(string permissionTypeCode, Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return Exclude(permissions => permissions.FilterByPermissionCode(permissionTypeCode), additionalFilter);
    }

    /// <summary>
    /// Removes permissions with the "Create" common permisison type from the collection.
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder ExcludeAllCreate(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return ExcludeAllWithPermissionType(CommonPermissionTypes.CreatePermissionCode, additionalFilter);
    }

    /// <summary>
    /// Removes permissions with the "Update" common permisison type from the collection.
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder ExcludeAllUpdate(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return ExcludeAllWithPermissionType(CommonPermissionTypes.UpdatePermissionCode, additionalFilter);
    }

    /// <summary>
    /// Removes permissions with the "Delete" common permisison type from the collection.
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder ExcludeAllDelete(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return ExcludeAllWithPermissionType(CommonPermissionTypes.DeletePermissionCode, additionalFilter);
    }

    /// <summary>
    /// Removes permissions with write common permisison types from the collection 
    /// which can include the "Create", "Update" and "Delete" permissions as well 
    /// as the more generic "Write" permission.
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder ExcludeAllWrite(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return Exclude(permissions => permissions.FilterToWritePermissions(), additionalFilter);
    }

    /// <summary>
    /// Removes permissions from the collection associated with a specific entity type.
    /// </summary>
    /// <typeparam name="TEntityDefinition">Definition type of the entity to remove from the collection</typeparam>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder ExcludeEntity<TEntityDefinition>(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
        where TEntityDefinition : IEntityDefinition
    {
        var entityDefiniton = _entityDefinitionRepository.GetRequired<TEntityDefinition>();
        return Exclude(permissions => permissions.FilterToEntityPermissions(entityDefiniton.EntityDefinitionCode), additionalFilter);
    }

    /// <summary>
    /// Filters a collection of permissions to only include permissions that permit
    /// access to sections in the admin panel. Specifically permissions that use the
    /// admin module common permission type code and the dashboard permission type code.
    /// </summary>
    /// <param name="permissionsToFilter">The collection of permissions to filter</param>
    /// <returns>Filtered collection of permissions</returns>
    public virtual IPermissionSetBuilder ExcludeAdminModule(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> additionalFilter = null)
    {
        return Include(permissions => permissions.FilterToAdminModulePermissions(), additionalFilter);
    }

    /// <summary>
    /// Used to detect circular deendencies when copying permissions
    /// from other roles.
    /// </summary>
    private class CircularDependencyGuard
    {
        private Type baseRoleType;
        private HashSet<Type> _executingRoles { get; } = new HashSet<Type>();

        public CircularDependencyGuard(IRoleDefinition roleDefinition)
        {
            baseRoleType = roleDefinition.GetType();
            _executingRoles.Add(baseRoleType);
        }

        public void AddRole<TRoleDefinition>()
            where TRoleDefinition : IRoleDefinition
        {
            var roleType = typeof(TRoleDefinition);
            if (_executingRoles.Contains(roleType))
            {
                var msg = $"Circular reference detected when copying permissions to {baseRoleType.Name}. Encountered circular dependency when referencing {roleType.Name}";
                throw new InvalidOperationException(msg);
            }
            _executingRoles.Add(roleType);
        }
    }
}
