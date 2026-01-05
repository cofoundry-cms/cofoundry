namespace Cofoundry.Domain;

/// <summary>
/// Extensions for <see cref="IPermissionSetBuilder"/>.
/// </summary>
public static class IPermissionSetBuilderExtensions
{
    extension(IPermissionSetBuilder builder)
    {
        /// <summary>
        /// Configure the builder to include the permissions configured for
        /// the <see cref="AnonymousRole"/>, which usually is a good base
        /// to start from before adding additional permissions.
        /// </summary>
        public IPermissionSetBuilder ApplyAnonymousRoleConfiguration()
        {
            return builder.ApplyRoleConfiguration<AnonymousRole>();
        }

        /// <summary>
        /// Configure the builder to include all permissions.
        /// </summary>
        public IPermissionSetBuilder IncludeAll()
        {
            return builder.Include(p => p);
        }
    }
}
