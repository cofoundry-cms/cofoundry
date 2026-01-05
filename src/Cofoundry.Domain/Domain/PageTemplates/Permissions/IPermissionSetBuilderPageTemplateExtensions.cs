using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// <see cref="IPermissionSetBuilder"/> extension methods for page templates.
/// </summary>
public static class IPermissionSetBuilderPageTemplateExtensions
{
    extension(IPermissionSetBuilder builder)
    {
        /// <summary>
        /// Configure the builder to include all permissions for page templates.
        /// </summary>
        public IPermissionSetBuilder IncludePageTemplate()
        {
            return Run(builder, null, true);
        }

        /// <summary>
        /// Configure the builder to include permissions for page templates.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to include.</param>
        public IPermissionSetBuilder IncludePageTemplate(Action<PageTemplatePermissionBuilder> configure)
        {
            return Run(builder, configure, true);
        }

        /// <summary>
        /// Configure the builder to exclude all permissions for page templates.
        /// </summary>
        public IPermissionSetBuilder ExcludePageTemplate()
        {
            return Run(builder, null, false);
        }

        /// <summary>
        /// Configure the builder to exclude permissions for page templates.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to exclude.</param>
        public IPermissionSetBuilder ExcludePageTemplate(Action<PageTemplatePermissionBuilder> configure)
        {
            return Run(builder, configure, false);
        }
    }

    private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<PageTemplatePermissionBuilder>? configure, bool isIncludeOperation)
    {
        if (configure == null)
        {
            configure = c => c.All();
        }

        var entityBuilder = new PageTemplatePermissionBuilder(builder, isIncludeOperation);
        configure(entityBuilder);

        return builder;
    }
}
