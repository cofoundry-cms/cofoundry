namespace Cofoundry.Domain;

public static class IPermissionSetBuilderExtensions
{
    /// <summary>
    /// Configure the builder to include the permissions configured for
    /// the <see cref="AnonymousRole"/>, which usually is a good base
    /// to start from before adding additional permissions.
    /// </summary>
    public static IPermissionSetBuilder ApplyAnonymousRoleConfiguration(this IPermissionSetBuilder builder)
    {
        return builder.ApplyRoleConfiguration<AnonymousRole>();
    }

    /// <summary>
    /// Configure the builder to include all permissions.
    /// </summary>
    public static IPermissionSetBuilder IncludeAll(this IPermissionSetBuilder builder)
    {
        return builder.Include(p => p);
    }
}
