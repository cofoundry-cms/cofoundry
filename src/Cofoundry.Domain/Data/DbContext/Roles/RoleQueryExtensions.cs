using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class RoleQueryExtensions
    {
        /// <summary>
        /// Filters the roles collection to include only the role
        /// with the specified id.
        /// </summary>
        /// <param name="roles">Collection of roles to filter.</param>
        /// <param name="id">Id of the role to filter to.</param>
        /// <returns>The filtered role collection.</returns>
        public static IQueryable<Role> FilterById(this IQueryable<Role> roles, int id)
        {
            var role = roles
                .Where(u => u.RoleId == id);

            return role;
        }
    }
}
