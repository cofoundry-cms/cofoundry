using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// <see cref="IPermissionSetBuilder"/> extension methods for the admin dashboard.
/// </summary>
public static class IPermissionSetBuilderDashboardExtensions
{
    extension(IPermissionSetBuilder builder)
    {
        /// <summary>
        /// Configure the builder to include all permissions for the admin dashboard.
        /// </summary>
        public IPermissionSetBuilder IncludeDashboard()
        {
            return Run(builder, null, true);
        }

        /// <summary>
        /// Configure the builder to include permissions for the admin dashboard.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to include.</param>
        public IPermissionSetBuilder IncludeDashboard(Action<DashboardPermissionBuilder> configure)
        {
            return Run(builder, configure, true);
        }

        /// <summary>
        /// Configure the builder to exclude all permissions for the admin dashboard.
        /// </summary>
        public IPermissionSetBuilder ExcludeDashboard()
        {
            return Run(builder, null, false);
        }

        /// <summary>
        /// Configure the builder to exclude permissions for the admin dashboard.
        /// </summary>
        /// <param name="configure">A configuration action to select which permissions to exclude.</param>
        public IPermissionSetBuilder ExcludeDashboard(Action<DashboardPermissionBuilder> configure)
        {
            return Run(builder, configure, false);
        }
    }

    private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<DashboardPermissionBuilder>? configure, bool isIncludeOperation)
    {
        if (configure == null)
        {
            configure = c => c.All();
        }

        var entityBuilder = new DashboardPermissionBuilder(builder, isIncludeOperation);
        configure(entityBuilder);

        return builder;
    }
}
