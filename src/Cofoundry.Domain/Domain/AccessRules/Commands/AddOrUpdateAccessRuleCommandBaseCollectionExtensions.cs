using Cofoundry.Core;
using System;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    public static class AddOrUpdateAccessRuleCommandBaseCollectionExtensions
    {
        /// <summary>
        /// Convenience method to add a new access rule to the <see cref="AccessRules"/> collection.
        /// </summary>
        /// <param name="userAreaCode">
        /// Unique 3 character code representing the user area to restrict
        /// the directory to. This cannot be the Cofoundry admin user area, as 
        /// access rules do not apply to admin panel users.
        /// </param>
        /// <param name="roleId">
        /// Optionally restrict access to a specific role within the selected 
        /// user area.
        /// </param>
        /// <returns>Command instance for method chaining.</returns>
        public static ICollection<TAddOrUpdateAccessRuleCommand> AddNew<TAddOrUpdateAccessRuleCommand>(this ICollection<TAddOrUpdateAccessRuleCommand> source, string userAreaCode, int? roleId = null)
            where TAddOrUpdateAccessRuleCommand : AddOrUpdateAccessRuleCommandBase, new()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (userAreaCode == null) throw new ArgumentNullException(nameof(userAreaCode));
            if (string.IsNullOrWhiteSpace(userAreaCode)) throw new ArgumentEmptyException(nameof(userAreaCode));

            source.Add(new TAddOrUpdateAccessRuleCommand()
            {
                UserAreaCode = userAreaCode,
                RoleId = roleId
            });

            return source;
        }
    }
}
