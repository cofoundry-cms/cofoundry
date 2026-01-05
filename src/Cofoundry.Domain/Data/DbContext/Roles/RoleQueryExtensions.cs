namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{Role}"/>.
/// </summary>
public static class RoleQueryExtensions
{
    extension(IQueryable<Role> roles)
    {
        /// <summary>
        /// Filters the roles collection to include only the role
        /// with the specified <paramref name="roleId"/>.
        /// </summary>
        /// <param name="roleId">Id of the role to filter to.</param>
        /// <returns>The filtered role collection.</returns>
        public IQueryable<Role> FilterById(int roleId)
        {
            var role = roles
                .Where(r => r.RoleId == roleId);

            return role;
        }

        /// <summary>
        /// Filters the roles collection to include only the role
        /// with the specified <paramref name="roleId"/> if one if defined,
        /// otherwise by <paramref name="roleCode"/>.
        /// </summary>
        /// <param name="roleId">Id of the role to filter to.</param>
        /// <param name="roleCode">3 character identifier of the role to filter to.</param>
        /// <returns>The filtered role collection.</returns>
        public IQueryable<Role> FilterByIdOrCode(int? roleId, string? roleCode)
        {
            if (roleId.HasValue)
            {
                return roles.FilterById(roleId.Value);
            }

            return roles.FilterByRoleCode(roleCode);
        }

        /// <summary>
        /// Filters the roles collection to include only the role
        /// with the specified <paramref name="roleCode"/>. Role codes are globally
        /// unique.
        /// </summary>
        /// <param name="roleCode">3 character identifier of the role to filter to.</param>
        /// <returns>The filtered role collection.</returns>
        public IQueryable<Role> FilterByRoleCode(string? roleCode)
        {
            var role = roles
                .Where(r => r.RoleCode != null && r.RoleCode == roleCode);

            return role;
        }
    }
}
