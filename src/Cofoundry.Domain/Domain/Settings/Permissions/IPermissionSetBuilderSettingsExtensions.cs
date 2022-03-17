using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

public static class IPermissionSetBuilderSettingsExtensions
{
    /// <summary>
    /// Configure the builder to include all permissions for settings.
    /// </summary>
    public static IPermissionSetBuilder IncludeSettings(this IPermissionSetBuilder builder)
    {
        return Run(builder, null, true);
    }

    /// <summary>
    /// Configure the builder to include permissions for settings.
    /// </summary>
    /// <param name="configure">A configuration action to select which permissions to include.</param>
    public static IPermissionSetBuilder IncludeSettings(this IPermissionSetBuilder builder, Action<SettingsPermissionBuilder> configure)
    {
        return Run(builder, configure, true);
    }

    /// <summary>
    /// Configure the builder to exclude all permissions for settings.
    /// </summary>
    public static IPermissionSetBuilder ExcludeSettings(this IPermissionSetBuilder builder)
    {
        return Run(builder, null, false);
    }

    /// <summary>
    /// Configure the builder to exclude permissions for settings.
    /// </summary>
    /// <param name="configure">A configuration action to select which permissions to exclude.</param>
    public static IPermissionSetBuilder ExcludeSettings(this IPermissionSetBuilder builder, Action<SettingsPermissionBuilder> configure)
    {
        return Run(builder, configure, false);
    }

    private static IPermissionSetBuilder Run(IPermissionSetBuilder builder, Action<SettingsPermissionBuilder> configure, bool isIncludeOperation)
    {
        if (configure == null) configure = c => c.All();
        var entityBuilder = new SettingsPermissionBuilder(builder, isIncludeOperation);
        configure(entityBuilder);

        return builder;
    }
}
