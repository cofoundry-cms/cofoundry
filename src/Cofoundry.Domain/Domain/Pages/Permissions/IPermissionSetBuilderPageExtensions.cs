using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// <see cref="IPermissionSetBuilder"/> extension methods for pages.
/// </summary>
public static class IPermissionSetBuilderPageExtensions
{
    extension(IPermissionSetBuilder builder)
    {
        /// <summary>
        /// Configure the builder to include all permissions for pages.
        /// </summary>
        public IPermissionSetBuilder IncludePage()
        {
            return Run(builder, null, true);
        }

        /// <summary>
        /// Configure the builder to include permissions for pages.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to include.</param>
        public IPermissionSetBuilder IncludePage(Action<PagePermissionBuilder> configure)
        {
            return Run(builder, configure, true);
        }

        /// <summary>
        /// Configure the builder to exclude all permissions for pages.
        /// </summary>
        public IPermissionSetBuilder ExcludePage()
        {
            return Run(builder, null, false);
        }

        /// <summary>
        /// Configure the builder to exclude permissions for pages.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to exclude.</param>
        public IPermissionSetBuilder ExcludePage(Action<PagePermissionBuilder> configure)
        {
            return Run(builder, configure, false);
        }
    }

    private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<PagePermissionBuilder>? configure, bool isIncludeOperation)
    {
        if (configure == null)
        {
            configure = c => c.All();
        }

        var entityBuilder = new PagePermissionBuilder(builder, isIncludeOperation);
        configure(entityBuilder);

        return builder;
    }
}
