using System.Linq;

namespace Cofoundry.Domain.Data
{
    public static class RoleQueryExtensions
    {
        /// <summary>
        /// Filters the roles collection to include only the role
        /// with the specified <paramref name="roleId"/>.
        /// </summary>
        /// <param name="roles">Collection of roles to filter.</param>
        /// <param name="roleId">Id of the role to filter to.</param>
        /// <returns>The filtered role collection.</returns>
        public static IQueryable<Role> FilterById(this IQueryable<Role> roles, int roleId)
        {
            var role = roles
                .Where(r => r.RoleId == roleId);

            return role;
        }

        /// <summary>
        /// Filters the roles collection to include only the role
        /// with the specified <paramref name="roleCode"/>. Role codes are globally
        /// unique.
        /// </summary>
        /// <param name="roles">Collection of roles to filter.</param>
        /// <param name="id">Id of the role to filter to.</param>
        /// <returns>The filtered role collection.</returns>
        public static IQueryable<Role> FilterByRoleCode(this IQueryable<Role> roles, string roleCode)
        {
            var role = roles
                .Where(r => r.RoleCode != null && r.RoleCode == roleCode);

            return role;
        }
    }
}
