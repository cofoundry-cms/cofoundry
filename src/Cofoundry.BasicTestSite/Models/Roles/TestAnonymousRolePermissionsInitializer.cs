using Cofoundry.Domain;

namespace Cofoundry.BasicTestSite
{
    /// <summary>
    /// We use a default initializer just for the anonymous role
    /// as it's the only built in role with permissions. This allows
    /// for you to implement a custom role initizlier for the anonymous
    /// role in your application to override the default one.
    /// </summary>
    public class TestAnonymousRolePermissionsInitializer : IAnonymousRolePermissionConfiguration
    {
        public void ConfigurePermissions(IPermissionSetBuilder builder)
        {
            builder
                .IncludeAnonymousRoleDefaults()
                .IncludeAllUpdate();
        }
    }
}
