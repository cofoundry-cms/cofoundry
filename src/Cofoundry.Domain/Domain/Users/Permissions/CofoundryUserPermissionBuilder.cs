using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to extend <see cref="IPermissionSetBuilder"/> to make it easier to
    /// include or exclude user permissions for the Cofoundry user area.
    /// </summary>
    public class CofoundryUserPermissionBuilder : EntityPermissionBuilder
    {
        public CofoundryUserPermissionBuilder(
            IPermissionSetBuilder permissionSetBuilder,
            bool isIncludeOperation
            )
            : base(permissionSetBuilder, isIncludeOperation)
        {
        }

        /// <summary>
        /// All permissions, including CRUD, special and admin module permissions.
        /// </summary>
        public CofoundryUserPermissionBuilder All()
        {
            return CRUD().Special().AdminModule();
        }

        /// <summary>
        /// Create, Read, Update and Delete permissions.
        /// </summary>
        public CofoundryUserPermissionBuilder CRUD()
        {
            return Create().Read().Update().Delete();
        }

        /// <summary>
        /// Special permissions: currently only ResetPassword.
        /// </summary>
        public CofoundryUserPermissionBuilder Special()
        {
            return ResetPassword();
        }

        /// <summary>
        /// Read access to all users in the Cofoundry admin user area. Read access is 
        /// required in order to include any other Cofoundry user permissions.
        /// </summary>
        public CofoundryUserPermissionBuilder Read()
        {
            Assign<CofoundryUserReadPermission>();
            return this;
        }

        /// <summary>
        /// Permission to create new users in the Cofoundry admin user area.
        /// </summary>
        public CofoundryUserPermissionBuilder Create()
        {
            Assign<CofoundryUserCreatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to update users in the Cofoundry admin user area, but not actions
        /// covered by special permissions such as issuing a password reset.
        /// </summary>
        public CofoundryUserPermissionBuilder Update()
        {
            Assign<CofoundryUserUpdatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to delete a user in the Cofoundry admin user area.
        /// </summary>
        public CofoundryUserPermissionBuilder Delete()
        {
            Assign<CofoundryUserDeletePermission>();
            return this;
        }

        /// <summary>
        /// Permission to access the Cofoundry users module in the admin panel.
        /// </summary>
        public CofoundryUserPermissionBuilder AdminModule()
        {
            Assign<CofoundryUserAdminModulePermission>();
            return this;
        }

        /// <summary>
        /// Permission to force a password reset for a user in the Cofoundry admin user area.
        /// </summary>
        public CofoundryUserPermissionBuilder ResetPassword()
        {
            Assign<CofoundryUserResetPasswordPermission>();
            return this;
        }
    }
}