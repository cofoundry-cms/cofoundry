using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public static class IPermissionExtensions
    {
        /// <summary>
        /// This hashset is used to group all the common permission types you'd associate 
        /// with "write" permissions.
        /// </summary>
        private static HashSet<string> writePermissionTypeCodes = new HashSet<string>() {
            { CommonPermissionTypes.WritePermissionCode },
            { CommonPermissionTypes.CreatePermissionCode },
            { CommonPermissionTypes.UpdatePermissionCode },
            { CommonPermissionTypes.DeletePermissionCode }
        };

        /// <summary>
        /// Creates a unique code that represents this permission by combining the
        /// entity definition code and the permission code. This logic is replicated
        /// on the Permission db entity.
        /// </summary>
        /// <param name="permission">Permission to get the unique code for</param>
        /// <returns>A string code that uniquely identifies this permission</returns>
        public static string GetUniqueCode(this IPermission permission)
        {
            if (permission.PermissionType == null) return null;
            string code;

            if (permission is IEntityPermission)
            {
                var customEntityPermission = (IEntityPermission)permission;
                if (customEntityPermission.EntityDefinition == null) return null;

                code = customEntityPermission.EntityDefinition.EntityDefinitionCode + permission.PermissionType.Code;
            }
            else
            {
                code = permission.PermissionType.Code;
            }

            return code?.ToUpperInvariant();
        }

        #region filters

        /// <summary>
        /// Filters a collection of permissions to only include IEntityPermission types.
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection cast to IEnumerable&lt;IEntityPermission&gt;</returns>
        public static IEnumerable<IEntityPermission> FilterEntityPermissions(this IEnumerable<IPermission> permissionsToFilter)
        {
            return permissionsToFilter
                .Where(p => p is IEntityPermission)
                .Cast<IEntityPermission>();
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions for a specific entity type.
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <param name="entityDefinitionCode">The definition code of the entity to filter on e.g. PageEntityDefinition.DefinitionCode</param>
        /// <returns>Filtered collection cast to IEnumerable&lt;IEntityPermission&gt;</returns>
        public static IEnumerable<IEntityPermission> FilterEntityPermissions(this IEnumerable<IPermission> permissionsToFilter, string entityDefinitionCode)
        {
            return permissionsToFilter
                .Where(p => p is IEntityPermission)
                .Cast<IEntityPermission>()
                .Where(p => p.EntityDefinition?.EntityDefinitionCode == entityDefinitionCode);
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions for a specific permission type
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <param name="permissionCode">The code of the permission type to filter on</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> FilterByPermissionCode(this IEnumerable<IPermission> permissionsToFilter, string permissionCode)
        {
            return permissionsToFilter.Where(p => p.PermissionType.Code == permissionCode);
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions that use the Read common permission type
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> FilterToReadPermissions(this IEnumerable<IPermission> permissionsToFilter)
        {
            return permissionsToFilter
                .FilterByPermissionCode(CommonPermissionTypes.ReadPermissionCode);
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions that use the Update common permission type
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> FilterToUpdatePermissions(this IEnumerable<IPermission> permissionsToFilter)
        {
            return permissionsToFilter
                .FilterByPermissionCode(CommonPermissionTypes.UpdatePermissionCode);
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions that use the Create common permission type
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> FilterToCreatePermissions(this IEnumerable<IPermission> permissionsToFilter)
        {
            return permissionsToFilter
                .FilterByPermissionCode(CommonPermissionTypes.CreatePermissionCode);
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions that use the Delete common permission type
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> FilterToDeletePermissions(this IEnumerable<IPermission> permissionsToFilter)
        {
            return permissionsToFilter
                .FilterByPermissionCode(CommonPermissionTypes.DeletePermissionCode);
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions that use common 
        /// permission types associated with writing data which can include the "Create", "Update" and "Delete"
        /// permissions as well as the more generic "Write" permission.
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> FilterToWritePermissions(this IEnumerable<IPermission> permissionsToFilter)
        {
            return permissionsToFilter
                .Where(p => writePermissionTypeCodes.Contains(p.PermissionType.Code));
        }

        /// <summary>
        /// Filters a collection of permissions to only include permissions that permit
        /// access to sections in the admin panel. Specifically permissions that use the
        /// admin module common permission type code and the dashboard permission type code.
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> FilterToAdminModulePermissions(this IEnumerable<IPermission> permissionsToFilter)
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
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> FilterToAnonymousRoleDefaults(this IEnumerable<IPermission> permissionsToFilter)
        {
            return permissionsToFilter
                .FilterToReadPermissions()
                .ExceptUserManagementPermissions();
        }

        #endregion

        #region exclusions

        /// <summary>
        /// Removes the specified permission from the collection.
        /// </summary>
        /// <typeparam name="TPermission">IPermission type to remove</typeparam>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> ExceptPermission<TPermission>(this IEnumerable<IPermission> permissionsToFilter)
        {
            return permissionsToFilter
                .Where(p => !(p is TPermission));
        }

        /// <summary>
        /// Removes permissions with the specified permission code from the collection.
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <param name="permissionCode">Code of the permission type to exclude from the collection</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> ExceptPermissionCode(this IEnumerable<IPermission> permissionsToFilter, string permissionCode)
        {
            return permissionsToFilter.Where(p => p.PermissionType.Code != permissionCode);
        }

        /// <summary>
        /// Removes permissions with the "Create" common permisison type from the collection.
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> ExceptCreatePermissions(this IEnumerable<IPermission> permissionsToFilter)
        {
            return permissionsToFilter
                .ExceptPermissionCode(CommonPermissionTypes.CreatePermissionCode);
        }

        /// <summary>
        /// Removes permissions with the "Update" common permisison type from the collection.
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> ExceptUpdatePermissions(this IEnumerable<IPermission> permissionsToFilter)
        {
            return permissionsToFilter
                .ExceptPermissionCode(CommonPermissionTypes.UpdatePermissionCode);
        }

        /// <summary>
        /// Removes permissions with the "Delete" common permisison type from the collection.
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> ExceptDeletePermissions(this IEnumerable<IPermission> permissionsToFilter)
        {
            return permissionsToFilter
                .ExceptPermissionCode(CommonPermissionTypes.DeletePermissionCode);
        }

        /// <summary>
        /// Removes permissions with write common permisison types from the collection 
        /// which can include the "Create", "Update" and "Delete" permissions as well 
        /// as the more generic "Write" permission.
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> ExceptWritePermissions(this IEnumerable<IPermission> permissionsToFilter)
        {
            return permissionsToFilter
                .Where(p => !writePermissionTypeCodes.Contains(p.PermissionType.Code));
        }

        /// <summary>
        /// Removes permissions from the collection associated with a specific entity type.
        /// </summary>
        /// <typeparam name="TEntityDefinition">Definition type of the entity to remove from the collection</typeparam>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> ExceptEntityPermissions<TEntityDefinition>(this IEnumerable<IPermission> permissionsToFilter)
            where TEntityDefinition : IEntityDefinition, new()
        {
            var entityDefiniton = new TEntityDefinition();

            return permissionsToFilter.ExceptEntityPermissions(entityDefiniton.EntityDefinitionCode);
        }

        /// <summary>
        /// Removes permissions from the collection associated with a specific entity type.
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <param name="entityDefinitionCode">The definition code of the entity to remove e.g. PageEntityDefinition.DefinitionCode</param>
        /// <returns></returns>
        public static IEnumerable<IPermission> ExceptEntityPermissions(this IEnumerable<IPermission> permissionsToFilter, string entityDefinitionCode)
        {
            return permissionsToFilter
                .Where(p => !(p is IEntityPermission) || ((IEntityPermission)p).EntityDefinition?.EntityDefinitionCode != entityDefinitionCode);
        }

        /// <summary>
        /// Removes all permissions for managing other user accounts. This does not
        /// exclude self-management user permissions.
        /// </summary>
        /// <param name="permissionsToFilter">The collection of permissions to filter</param>
        /// <returns>Filtered collection of permissions</returns>
        public static IEnumerable<IPermission> ExceptUserManagementPermissions(this IEnumerable<IPermission> permissionsToFilter)
        {
            return permissionsToFilter
                .ExceptEntityPermissions(UserEntityDefinition.DefinitionCode)
                .ExceptEntityPermissions(NonCofoundryUserEntityDefinition.DefinitionCode);
        }

        #endregion
    }
}
