using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPermissionSetBuilder"/>.
/// </summary>
public class PermissionSetBuilder : IExtendablePermissionSetBuilder
{
    private readonly PermissionEqualityComparer _permissionEqualityComparer = new();
    private IEnumerable<IPermission> _permissions = Array.Empty<IPermission>();
    private readonly CircularDependencyGuard? _circularDependencyGuard;

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

    /// <inheritdoc/>
    public IEnumerable<IPermission> AvailablePermissions { get; }

    /// <inheritdoc/>
    public IServiceProvider ServiceProvider { get; }

    /// <inheritdoc/>
    public IReadOnlyCollection<IPermission> Build()
    {
        return _permissions.ToArray();
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder Include(IEnumerable<IPermission> permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);

        return Include(p => p.Intersect(permissions, _permissionEqualityComparer));
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder Include(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> permissionFilter)
    {
        ArgumentNullException.ThrowIfNull(permissionFilter);

        var filteredPermissions = permissionFilter(AvailablePermissions);
        _permissions = _permissions.Union(filteredPermissions, _permissionEqualityComparer);

        return this;
    }

    private IPermissionSetBuilder Include(
        Func<IEnumerable<IPermission>, IEnumerable<IPermission>> permissionFilter,
        Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter
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

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder Exclude(IEnumerable<IPermission> permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);

        return Exclude(p => p.Intersect(permissions, _permissionEqualityComparer));
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder Exclude(Func<IEnumerable<IPermission>, IEnumerable<IPermission>> permissionFilter)
    {
        ArgumentNullException.ThrowIfNull(permissionFilter);

        var permissionsToExclude = permissionFilter(AvailablePermissions);
        _permissions = _permissions.Except(permissionsToExclude, _permissionEqualityComparer);

        return this;
    }

    private IPermissionSetBuilder Exclude(
        Func<IEnumerable<IPermission>, IEnumerable<IPermission>> permissionFilter,
        Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter
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

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder ApplyRoleConfiguration<TRoleDefinition>(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
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

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder IncludeEntity<TEntity>(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
        where TEntity : IEntityDefinition
    {
        var entityDefinition = _entityDefinitionRepository.GetRequired<TEntity>();
        return Include(permissions => permissions.FilterToEntityPermissions(entityDefinition.EntityDefinitionCode), additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder IncludeAllWithPermissionType(string permissionTypeCode, Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return Include(permissions => permissions.Where(p => p.PermissionType.Code == permissionTypeCode), additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder Include<TPermission>()
        where TPermission : IPermission
    {
        return Include(permissions => permissions.Where(p => p is TPermission));
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder IncludeAllRead(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return IncludeAllWithPermissionType(CommonPermissionTypes.ReadPermissionCode, additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder IncludeAllUpdate(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return IncludeAllWithPermissionType(CommonPermissionTypes.UpdatePermissionCode, additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder IncludeAllCreate(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return IncludeAllWithPermissionType(CommonPermissionTypes.CreatePermissionCode, additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder IncludeAllDelete(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return IncludeAllWithPermissionType(CommonPermissionTypes.DeletePermissionCode, additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder IncludeAllWrite(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return Include(permissions => permissions.FilterToWritePermissions(), additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder IncludeAllAdminModule(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return Include(permissions => permissions.FilterToAdminModulePermissions(), additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder IncludeAnonymousRoleDefaults(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return Include(permissions => permissions.FilterToAnonymousRoleDefaults(), additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder Exclude<TPermission>()
    {
        return Exclude(permissions => permissions.Where(p => p is TPermission));
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder ExcludeAllWithPermissionType(string permissionTypeCode, Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return Exclude(permissions => permissions.FilterByPermissionCode(permissionTypeCode), additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder ExcludeAllCreate(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return ExcludeAllWithPermissionType(CommonPermissionTypes.CreatePermissionCode, additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder ExcludeAllUpdate(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return ExcludeAllWithPermissionType(CommonPermissionTypes.UpdatePermissionCode, additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder ExcludeAllDelete(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return ExcludeAllWithPermissionType(CommonPermissionTypes.DeletePermissionCode, additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder ExcludeAllWrite(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return Exclude(permissions => permissions.FilterToWritePermissions(), additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder ExcludeEntity<TEntityDefinition>(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
        where TEntityDefinition : IEntityDefinition
    {
        var entityDefiniton = _entityDefinitionRepository.GetRequired<TEntityDefinition>();
        return Exclude(permissions => permissions.FilterToEntityPermissions(entityDefiniton.EntityDefinitionCode), additionalFilter);
    }

    /// <inheritdoc/>
    public virtual IPermissionSetBuilder ExcludeAdminModule(Func<IEnumerable<IPermission>, IEnumerable<IPermission>>? additionalFilter = null)
    {
        return Include(permissions => permissions.FilterToAdminModulePermissions(), additionalFilter);
    }

    /// <summary>
    /// Used to detect circular deendencies when copying permissions
    /// from other roles.
    /// </summary>
    private class CircularDependencyGuard
    {
        private readonly Type _baseRoleType;
        private HashSet<Type> _executingRoles { get; } = [];

        public CircularDependencyGuard(IRoleDefinition roleDefinition)
        {
            _baseRoleType = roleDefinition.GetType();
            _executingRoles.Add(_baseRoleType);
        }

        public void AddRole<TRoleDefinition>()
            where TRoleDefinition : IRoleDefinition
        {
            var roleType = typeof(TRoleDefinition);
            if (_executingRoles.Contains(roleType))
            {
                var msg = $"Circular reference detected when copying permissions to {_baseRoleType.Name}. Encountered circular dependency when referencing {roleType.Name}";
                throw new InvalidOperationException(msg);
            }
            _executingRoles.Add(roleType);
        }
    }
}
