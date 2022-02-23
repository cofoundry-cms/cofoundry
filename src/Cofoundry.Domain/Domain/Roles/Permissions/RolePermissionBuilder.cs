using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to extend <see cref="IPermissionSetBuilder"/> to make it easier to
    /// include or exclude role permissions.
    /// </summary>
    public class RolePermissionBuilder : EntityPermissionBuilder
    {
        public RolePermissionBuilder(
            IPermissionSetBuilder permissionSetBuilder,
            bool isIncludeOperation
            )
            : base(permissionSetBuilder, isIncludeOperation)
        {
        }

        /// <summary>
        /// All permissions, including CRUD and admin module permissions.
        /// </summary>
        public RolePermissionBuilder All()
        {
            return CRUD().AdminModule();
        }

        /// <summary>
        /// Create, Read, Update and Delete permissions.
        /// </summary>
        public RolePermissionBuilder CRUD()
        {
            return Create().Read().Update().Delete();
        }

        /// <summary>
        /// Read access to roles. Read access is required in order
        /// to include any other role permissions.
        /// </summary>
        public RolePermissionBuilder Read()
        {
            Assign<RoleReadPermission>();
            return this;
        }

        /// <summary>
        /// Permission to create new roles.
        /// </summary>
        public RolePermissionBuilder Create()
        {
            Assign<RoleCreatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to update a role.
        /// </summary>
        public RolePermissionBuilder Update()
        {
            Assign<RoleUpdatePermission>();
            return this;
        }

        /// <summary>
        /// Permission to delete a role.
        /// </summary>
        public RolePermissionBuilder Delete()
        {
            Assign<RoleDeletePermission>();
            return this;
        }

        /// <summary>
        /// Permission to access the roles module in the admin panel.
        /// </summary>
        public RolePermissionBuilder AdminModule()
        {
            Assign<RoleAdminModulePermission>();
            return this;
        }
    }
}