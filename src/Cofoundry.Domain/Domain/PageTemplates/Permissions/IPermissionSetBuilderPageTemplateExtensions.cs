using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

public static class IPermissionSetBuilderPageTemplateExtensions
{
    /// <summary>
    /// Configure the builder to include all permissions for page templates.
    /// </summary>
    public static IPermissionSetBuilder IncludePageTemplate(this IPermissionSetBuilder builder)
    {
        return Run(builder, null, true);
    }

    /// <summary>
    /// Configure the builder to include permissions for page templates.
    /// </summary>
    /// <param name="configure">A configuration action to select which permissions to include.</param>
    public static IPermissionSetBuilder IncludePageTemplate(this IPermissionSetBuilder builder, Action<PageTemplatePermissionBuilder> configure)
    {
        return Run(builder, configure, true);
    }

    /// <summary>
    /// Configure the builder to exclude all permissions for page templates.
    /// </summary>
    public static IPermissionSetBuilder ExcludePageTemplate(this IPermissionSetBuilder builder)
    {
        return Run(builder, null, false);
    }

    /// <summary>
    /// Configure the builder to exclude permissions for page templates.
    /// </summary>
    /// <param name="configure">A configuration action to select which permissions to exclude.</param>
    public static IPermissionSetBuilder ExcludePageTemplate(this IPermissionSetBuilder builder, Action<PageTemplatePermissionBuilder> configure)
    {
        return Run(builder, configure, false);
    }

    private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<PageTemplatePermissionBuilder> configure, bool isIncludeOperation)
    {
        if (configure == null) configure = c => c.All();
        var entityBuilder = new PageTemplatePermissionBuilder(builder, isIncludeOperation);
        configure(entityBuilder);

        return builder;
    }
}
