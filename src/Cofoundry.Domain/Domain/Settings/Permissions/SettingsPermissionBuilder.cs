using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to extend <see cref="IPermissionSetBuilder"/> to make it easier to
    /// include or exclude permissions for settings.
    /// </summary>
    public class SettingsPermissionBuilder : EntityPermissionBuilder
    {
        public SettingsPermissionBuilder(
            IPermissionSetBuilder permissionSetBuilder,
            bool isIncludeOperation
            )
            : base(permissionSetBuilder, isIncludeOperation)
        {
        }

        /// <summary>
        /// All permissions, including CRUD and admin module permissions.
        /// </summary>
        public SettingsPermissionBuilder All()
        {
            return UpdateGeneral().UpdateSEO().AdminModule();
        }

        /// <summary>
        /// Permission to update general settings.
        /// </summary>
        public SettingsPermissionBuilder UpdateGeneral()
        {
            Assign<GeneralSettingsUpdatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to update SEO settings.
        /// </summary>
        public SettingsPermissionBuilder UpdateSEO()
        {
            Assign<SeoSettingsUpdatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to access the settings module in the admin panel.
        /// </summary>
        public SettingsPermissionBuilder AdminModule()
        {
            Assign<SettingsAdminModulePermission>();
            return this;
        }
    }
}