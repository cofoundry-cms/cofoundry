namespace Cofoundry.Domain;

/// <summary>
/// Extensions for filtering collections of <see cref="IPermission"/>.
/// </summary>
public static class IPermissionCollectionExtensions
{
    /// <summary>
    /// This hashset is used to group all the common permission types you'd associate 
    /// with "write" permissions.
    /// </summary>
    private static readonly HashSet<string> _writePermissionTypeCodes = new() {
        { CommonPermissionTypes.WritePermissionCode },
        { CommonPermissionTypes.CreatePermissionCode },
        { CommonPermissionTypes.UpdatePermissionCode },
        { CommonPermissionTypes.DeletePermissionCode }
    };

    extension(IEnumerable<IPermission> permissionsToFilter)
    {
        /// <summary>
        /// Filters a collection of permissions to only include <see cref="IEntityPermission"/> types.
        /// </summary>
        /// <returns>Filtered collection cast to <see cref="IEnumerable{IEntityPermission}"/></returns>
        public IEnumerable<IEntityPermission> FilterToEntityPermissions()
        {
            return permissionsToFilter
                .Where(p => p is IEntityPermission)
                .Cast<IEntityPermission>();
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions for a specific entity type.
        /// </summary>
        /// <param name="entityDefinitionCode">The definition code of the entity to filter on e.g. <see cref="PageEntityDefinition.DefinitionCode"/></param>
        /// <returns>Filtered collection cast to <see cref="IEnumerable{IEntityPermission}"/></returns>
        public IEnumerable<IEntityPermission> FilterToEntityPermissions(string entityDefinitionCode)
        {
            return permissionsToFilter
                .Where(p => p is IEntityPermission)
                .Cast<IEntityPermission>()
                .Where(p => p.EntityDefinition?.EntityDefinitionCode == entityDefinitionCode);
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions for a specific permission type
        /// </summary>
        /// <param name="permissionCode">The code of the permission type to filter on</param>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> FilterByPermissionCode(string permissionCode)
        {
            return permissionsToFilter.Where(p => p.PermissionType.Code == permissionCode);
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions that are or inherit from a specific permission type
        /// </summary>
        /// <typeparam name="TPermission">The type of permission to filter on.</typeparam>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> FilterByType<TPermission>()
        {
            return permissionsToFilter.Where(p => p is TPermission);
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions that use the Read common permission type
        /// </summary>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> FilterToReadPermissions()
        {
            return permissionsToFilter.FilterByPermissionCode(CommonPermissionTypes.ReadPermissionCode);
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions that use the Update common permission type
        /// </summary>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> FilterToUpdatePermissions()
        {
            return permissionsToFilter.FilterByPermissionCode(CommonPermissionTypes.UpdatePermissionCode);
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions that use the Create common permission type
        /// </summary>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> FilterToCreatePermissions()
        {
            return permissionsToFilter.FilterByPermissionCode(CommonPermissionTypes.CreatePermissionCode);
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions that use the Delete common permission type
        /// </summary>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> FilterToDeletePermissions()
        {
            return permissionsToFilter.FilterByPermissionCode(CommonPermissionTypes.DeletePermissionCode);
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions that use common 
        /// permission types associated with writing data which can include the "Create", "Update" and "Delete"
        /// permissions as well as the more generic "Write" permission.
        /// </summary>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> FilterToWritePermissions()
        {
            return permissionsToFilter.Where(p => _writePermissionTypeCodes.Contains(p.PermissionType.Code));
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions that permit
        /// access to sections in the admin panel. Specifically permissions that use the
        /// admin module common permission type code and the dashboard permission type code.
        /// </summary>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> FilterToAdminModulePermissions()
        {
            return permissionsToFilter
                .Where(p => p.PermissionType.Code == CommonPermissionTypes.AdminModulePermissionCode
                    || p.PermissionType.Code == DashboardAdminModulePermission.PermissionTypeCode);
        }

        /// <summary>
        /// The anonymous role by default can read any entity except for users.
        /// This is because user read permission means 'all users' not just 'current user'
        /// and is associated with user management.
        /// </summary>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> FilterToAnonymousRoleDefaults()
        {
            return permissionsToFilter
                .FilterToReadPermissions()
                .ExceptUserManagementPermissions();
        }

        /// <summary>
        /// Removes the specified permission from the collection.
        /// </summary>
        /// <typeparam name="TPermission">IPermission type to remove</typeparam>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> ExceptPermission<TPermission>()
        {
            return permissionsToFilter.Where(p => p is not TPermission);
        }

        /// <summary>
        /// Removes permissions with the specified permission code from the collection.
        /// </summary>
        /// <param name="permissionCode">Code of the permission type to exclude from the collection</param>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> ExceptPermissionCode(string permissionCode)
        {
            return permissionsToFilter.Where(p => p.PermissionType.Code != permissionCode);
        }

        /// <summary>
        /// Removes permissions with the "Create" common permisison type from the collection.
        /// </summary>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> ExceptCreatePermissions()
        {
            return permissionsToFilter.ExceptPermissionCode(CommonPermissionTypes.CreatePermissionCode);
        }

        /// <summary>
        /// Removes permissions with the "Update" common permisison type from the collection.
        /// </summary>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> ExceptUpdatePermissions()
        {
            return permissionsToFilter.ExceptPermissionCode(CommonPermissionTypes.UpdatePermissionCode);
        }

        /// <summary>
        /// Removes permissions with the "Delete" common permisison type from the collection.
        /// </summary>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> ExceptDeletePermissions()
        {
            return permissionsToFilter.ExceptPermissionCode(CommonPermissionTypes.DeletePermissionCode);
        }

        /// <summary>
        /// Removes permissions with write common permisison types from the collection 
        /// which can include the "Create", "Update" and "Delete" permissions as well 
        /// as the more generic "Write" permission.
        /// </summary>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> ExceptWritePermissions()
        {
            return permissionsToFilter.Where(p => !_writePermissionTypeCodes.Contains(p.PermissionType.Code));
        }

        /// <summary>
        /// Removes permissions from the collection associated with a specific entity type.
        /// </summary>
        /// <typeparam name="TEntityDefinition">Definition type of the entity to remove from the collection</typeparam>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> ExceptEntityPermissions<TEntityDefinition>()
            where TEntityDefinition : IEntityDefinition, new()
        {
            var entityDefiniton = new TEntityDefinition();

            return permissionsToFilter.ExceptEntityPermissions(entityDefiniton.EntityDefinitionCode);
        }

        /// <summary>
        /// Removes permissions from the collection associated with a specific entity type.
        /// </summary>
        /// <param name="entityDefinitionCode">The definition code of the entity to remove e.g. PageEntityDefinition.DefinitionCode</param>
        /// <returns></returns>
        public IEnumerable<IPermission> ExceptEntityPermissions(string entityDefinitionCode)
        {
            return permissionsToFilter.Where(p =>
                p is not IEntityPermission
                || ((IEntityPermission)p).EntityDefinition?.EntityDefinitionCode != entityDefinitionCode);
        }

        /// <summary>
        /// Filters a collection of permissions to exclude permissions to
        /// access to sections in the admin panel. Specifically permissions that use the
        /// admin module common permission type code and the dashboard permission type code.
        /// </summary>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> ExceptAdminModulePermissions()
        {
            return permissionsToFilter.Where(p =>
                p.PermissionType.Code != CommonPermissionTypes.AdminModulePermissionCode
                && p.PermissionType.Code != DashboardAdminModulePermission.PermissionTypeCode);
        }

        /// <summary>
        /// Removes all permissions for managing other user accounts. This does not
        /// exclude self-management user permissions.
        /// </summary>
        /// <returns>Filtered collection of permissions</returns>
        public IEnumerable<IPermission> ExceptUserManagementPermissions()
        {
            return permissionsToFilter
                .ExceptEntityPermissions(UserEntityDefinition.DefinitionCode)
                .ExceptEntityPermissions(NonCofoundryUserEntityDefinition.DefinitionCode);
        }
    }
}
