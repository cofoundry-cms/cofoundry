namespace Cofoundry.Domain
{
    public interface IRolePermissionInitializer
    {
        /// <summary>
        /// Configures an <see cref="IPermissionSetBuilder"/> with a set of permissions
        /// for a specified role. This is a thin abstraction over <see cref="IRoleDefinition.ConfigurePermissions"/>
        /// that allows a custom implementaton for the anonymous role to override the defaults.
        /// </summary>
        /// <param name="permissionSetBuilder">The builder to configure with permissions.</param>
        void Initialize(IPermissionSetBuilder permissionSetBuilder);
    }
}