namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class RolePermissionInitializerFactory : IRolePermissionInitializerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public RolePermissionInitializerFactory(
        IServiceProvider serviceProvider
        )
    {
        _serviceProvider = serviceProvider;
    }

    public IRolePermissionInitializer Create(IRoleDefinition roleDefinition)
    {
        if (roleDefinition is AnonymousRole)
        {
            return new AnonymousRolePermissionInitializer(_serviceProvider);
        }
        else
        {
            return new RoleDefinitionRolePermissionInitializer(roleDefinition);
        }
    }
}
