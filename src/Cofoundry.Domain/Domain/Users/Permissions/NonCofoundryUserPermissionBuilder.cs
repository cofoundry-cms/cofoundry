using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to extend <see cref="IPermissionSetBuilder"/> to make it easier to
    /// include or exclude user permissions for custom user areas (excludes the Cofoundry admin user area).
    /// </summary>
    public class NonCofoundryUserPermissionBuilder : EntityPermissionBuilder
    {
        public NonCofoundryUserPermissionBuilder(
            IPermissionSetBuilder permissionSetBuilder,
            bool isIncludeOperation
            )
            : base(permissionSetBuilder, isIncludeOperation)
        {
        }

        /// <summary>
        /// All permissions, including CRUD, special and admin module permissions.
        /// </summary>
        public NonCofoundryUserPermissionBuilder All()
        {
            return CRUD().Special().AdminModule();
        }

        /// <summary>
        /// Create, Read, Update and Delete permissions.
        /// </summary>
        public NonCofoundryUserPermissionBuilder CRUD()
        {
            return Create().Read().Update().Delete();
        }

        /// <summary>
        /// Special permissions: currently only ResetPassword.
        /// </summary>
        public NonCofoundryUserPermissionBuilder Special()
        {
            return ResetPassword();
        }

        /// <summary>
        /// Read access to all users in custom user areas (excludes the Cofoundry admin user area). Read access is 
        /// required in order to include any other permissions.
        /// </summary>
        public NonCofoundryUserPermissionBuilder Read()
        {
            Assign<NonCofoundryUserReadPermission>();
            return this;
        }

        /// <summary>
        /// Permission to create new users in custom user areas (excludes the Cofoundry admin user area).
        /// </summary>
        public NonCofoundryUserPermissionBuilder Create()
        {
            Assign<NonCofoundryUserCreatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to update users in custom user areas (excludes the Cofoundry admin user area), but not actions
        /// covered by special permissions such as issuing a password reset.
        /// </summary>
        public NonCofoundryUserPermissionBuilder Update()
        {
            Assign<NonCofoundryUserUpdatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to delete a user in custom user areas (excludes the Cofoundry admin user area).
        /// </summary>
        public NonCofoundryUserPermissionBuilder Delete()
        {
            Assign<NonCofoundryUserDeletePermission>();
            return this;
        }

        /// <summary>
        /// Permission to access the users module in the admin panel for custom user areas (excludes the Cofoundry admin user area).
        /// </summary>
        public NonCofoundryUserPermissionBuilder AdminModule()
        {
            Assign<NonCofoundryUserAdminModulePermission>();
            return this;
        }

        /// <summary>
        /// Permission to force a password reset for users in custom user areas (excludes the Cofoundry admin user area).
        /// </summary>
        public NonCofoundryUserPermissionBuilder ResetPassword()
        {
            Assign<NonCofoundryUserResetPasswordPermission>();
            return this;
        }
    }
}