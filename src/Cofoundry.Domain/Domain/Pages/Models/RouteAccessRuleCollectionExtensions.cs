using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain
{
    public static class RouteAccessRuleCollectionExtensions
    {
        /// <summary>
        /// Filters the collection to include only rules associated with the
        /// <paramref name="role"/>.
        /// </summary>
        /// <param name="accessRules">The <see cref="RouteAccessRule"/> collection to filer.</param>
        /// <param name="role">The role to filter on.</param>
        /// <returns>A collection of matching rules.</returns>
        public static IEnumerable<RouteAccessRule> FilterByRole(this IEnumerable<RouteAccessRule> accessRules, RoleDetails role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            return accessRules
                .Where(r => r.UserAreaCode == role.UserArea.UserAreaCode || r.RoleId == role.RoleId);
        }
    }
}
