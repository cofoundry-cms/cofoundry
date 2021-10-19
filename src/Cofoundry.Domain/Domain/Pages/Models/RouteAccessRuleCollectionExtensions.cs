using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain
{
    public static class RouteAccessRuleCollectionExtensions
    {
        /// <summary>
        /// Filters the collection to include only rules not matched by the
        /// <paramref name="user"/>.
        /// </summary>
        /// <param name="accessRules">The <see cref="RouteAccessRule"/> collection to filer.</param>
        /// <param name="userContext">The <see cref="IUserContext"/> to filter on.</param>
        /// <returns>A collection of rules that don't match the specified user account.</returns>
        public static IEnumerable<RouteAccessRule> GetRuleViolations(this IEnumerable<RouteAccessRule> accessRules, IUserContext userContext)
        {
            if (userContext == null) throw new ArgumentNullException(nameof(userContext));

            return accessRules
                .Where(r => r.UserAreaCode != userContext.UserArea?.UserAreaCode 
                    || (r.RoleId.HasValue && r.RoleId != userContext.RoleId));
        }
    }
}
