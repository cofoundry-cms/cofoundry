using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Domain;

public static class IPermissionSetBuilderCustomEntityExtensions
{
    /// <summary>
    /// Configure the builder to include all permissions for a specific custom entity type.
    /// </summary>
    public static IPermissionSetBuilder IncludeCustomEntity<TCustomEntityDefinition>(this IPermissionSetBuilder builder)
        where TCustomEntityDefinition : ICustomEntityDefinition
    {
        return Run<TCustomEntityDefinition>(builder, null, true);
    }

    /// <summary>
    /// Configure the builder to include permissions for a custom entity type.
    /// </summary>
    /// <param name="configure">A configuration action to select which permissions to include.</param>
    public static IPermissionSetBuilder IncludeCustomEntity<TCustomEntityDefinition>(this IPermissionSetBuilder builder, Action<CustomEntityPermissionBuilder> configure)
        where TCustomEntityDefinition : ICustomEntityDefinition
    {
        return Run<TCustomEntityDefinition>(builder, configure, true);
    }

    /// <summary>
    /// Configure the builder to exclude all permissions for a specific custom entity type.
    /// </summary>
    public static IPermissionSetBuilder ExcludeCustomEntity<TCustomEntityDefinition>(this IPermissionSetBuilder builder)
        where TCustomEntityDefinition : ICustomEntityDefinition
    {
        return Run<TCustomEntityDefinition>(builder, null, false);
    }

    /// <summary>
    /// Configure the builder to exclude permissions for a custom entity type.
    /// </summary>
    /// <param name="configure">A configuration action to select which permissions to exclude.</param>
    public static IPermissionSetBuilder ExcludeCustomEntity<TCustomEntityDefinition>(this IPermissionSetBuilder builder, Action<CustomEntityPermissionBuilder> configure)
        where TCustomEntityDefinition : ICustomEntityDefinition
    {
        return Run<TCustomEntityDefinition>(builder, configure, false);
    }

    private static IPermissionSetBuilder Run<TCustomEntityDefinition>(IPermissionSetBuilder builder, Action<CustomEntityPermissionBuilder> configure, bool isIncludeOperation)
        where TCustomEntityDefinition : ICustomEntityDefinition
    {
        if (configure == null) configure = c => c.All();
        var definition = GetDefinition<TCustomEntityDefinition>(builder);
        var entityBuilder = new CustomEntityPermissionBuilder(definition, builder, isIncludeOperation);
        configure(entityBuilder);

        return builder;
    }

    private static ICustomEntityDefinition GetDefinition<TCustomEntityDefinition>(IPermissionSetBuilder builder)
        where TCustomEntityDefinition : ICustomEntityDefinition
    {
        var extendableBuilder = builder.AsExtendableBuilder();
        var repository = extendableBuilder.ServiceProvider.GetRequiredService<ICustomEntityDefinitionRepository>();
        var definition = repository.GetRequired<TCustomEntityDefinition>();

        return definition;
    }
}
