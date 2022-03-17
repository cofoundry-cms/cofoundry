using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

public static class IPermissionSetBuilderPageDirectoryExtensions
{
    /// <summary>
    /// Configure the builder to include all permissions for page directories.
    /// </summary>
    public static IPermissionSetBuilder IncludePageDirectory(this IPermissionSetBuilder builder)
    {
        return Run(builder, null, true);
    }

    /// <summary>
    /// Configure the builder to include permissions for page directory entities.
    /// </summary>
    /// <param name="configure">A configuration action to select which permissions to include.</param>
    public static IPermissionSetBuilder IncludePageDirectory(this IPermissionSetBuilder builder, Action<PageDirectoryPermissionBuilder> configure)
    {
        return Run(builder, configure, true);
    }

    /// <summary>
    /// Configure the builder to exclude all permissions for page directories.
    /// </summary>
    public static IPermissionSetBuilder ExcludePageDirectory(this IPermissionSetBuilder builder)
    {
        return Run(builder, null, false);
    }

    /// <summary>
    /// Configure the builder to exclude permissions for page directory entities.
    /// </summary>
    /// <param name="configure">A configuration action to select which permissions to exclude.</param>
    public static IPermissionSetBuilder ExcludePageDirectory(this IPermissionSetBuilder builder, Action<PageDirectoryPermissionBuilder> configure)
    {
        return Run(builder, configure, false);
    }

    private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<PageDirectoryPermissionBuilder> configure, bool isIncludeOperation)
    {
        if (configure == null) configure = c => c.All();
        var entityBuilder = new PageDirectoryPermissionBuilder(builder, isIncludeOperation);
        configure(entityBuilder);

        return builder;
    }
}
