using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to extend <see cref="IPermissionSetBuilder"/> to make it easier to
    /// include or exclude dashboard permissions.
    /// </summary>
    public class DashboardPermissionBuilder : EntityPermissionBuilder
    {
        public DashboardPermissionBuilder(
            IPermissionSetBuilder permissionSetBuilder,
            bool isIncludeOperation
            )
            : base(permissionSetBuilder, isIncludeOperation)
        {
        }

        /// <summary>
        /// All permissions, which currently is just the admin module permissions.
        /// </summary>
        public DashboardPermissionBuilder All()
        {
            return AdminModule();
        }

        /// <summary>
        /// Permission to access the dashboards module in the admin panel.
        /// </summary>
        public DashboardPermissionBuilder AdminModule()
        {
            Assign<DashboardAdminModulePermission>();
            return this;
        }
    }
}