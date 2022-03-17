namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class PermissionSetBuilderFactory : IPermissionSetBuilderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRolePermissionInitializerFactory _rolePermissionInitializerFactory;
    private readonly IEntityDefinitionRepository _entityDefinitionRepository;
    private readonly IRoleDefinitionRepository _roleDefinitionRepository;

    public PermissionSetBuilderFactory(
        IServiceProvider serviceProvider,
        IRolePermissionInitializerFactory rolePermissionInitializerFactory,
        IEntityDefinitionRepository entityDefinitionRepository,
        IRoleDefinitionRepository roleDefinitionRepository
        )
    {
        _serviceProvider = serviceProvider;
        _rolePermissionInitializerFactory = rolePermissionInitializerFactory;
        _entityDefinitionRepository = entityDefinitionRepository;
        _roleDefinitionRepository = roleDefinitionRepository;
    }

    public IPermissionSetBuilder Create(IEnumerable<IPermission> permissionsToFilter)
    {
        return new PermissionSetBuilder(
            permissionsToFilter,
            _serviceProvider,
            _rolePermissionInitializerFactory,
            _entityDefinitionRepository,
            _roleDefinitionRepository
            );
    }
}
