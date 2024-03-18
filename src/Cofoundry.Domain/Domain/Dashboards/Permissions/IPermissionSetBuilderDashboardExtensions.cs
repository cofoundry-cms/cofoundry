﻿using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

public static class IPermissionSetBuilderDashboardExtensions
{
    /// <summary>
    /// Configure the builder to include all permissions for the admin dashboard.
    /// </summary>
    public static IPermissionSetBuilder IncludeDashboard(this IPermissionSetBuilder builder)
    {
        return Run(builder, null, true);
    }

    /// <summary>
    /// Configure the builder to include permissions for the admin dashboard.
    /// </summary>
    /// <param name="configure">A configuration action to select which permissions to include.</param>
    public static IPermissionSetBuilder IncludeDashboard(this IPermissionSetBuilder builder, Action<DashboardPermissionBuilder> configure)
    {
        return Run(builder, configure, true);
    }

    /// <summary>
    /// Configure the builder to exclude all permissions for the admin dashboard.
    /// </summary>
    public static IPermissionSetBuilder ExcludeDashboard(this IPermissionSetBuilder builder)
    {
        return Run(builder, null, false);
    }

    /// <summary>
    /// Configure the builder to exclude permissions for the admin dashboard.
    /// </summary>
    /// <param name="configure">A configuration action to select which permissions to exclude.</param>
    public static IPermissionSetBuilder ExcludeDashboard(this IPermissionSetBuilder builder, Action<DashboardPermissionBuilder> configure)
    {
        return Run(builder, configure, false);
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
