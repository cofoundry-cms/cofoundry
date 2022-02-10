using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// A set of access rules for an entity that can form a resource route or part of 
    /// a route in a website, such as a page or page directory. A full resource path
    /// can contain multiple access rule sets e.g. one for each directory in a nested 
    /// path.
    /// </para>
    /// <para>
    /// This is a generic and lightweight representation designed to be used in 
    /// routing, so it only contains the bare minimum fields required to make an 
    /// access decision.
    /// </para>
    /// </summary>
    public class EntityAccessRuleSet
    {
        /// <summary>
        /// Each entity can have zero or more access rules. In order to pass the
        /// authorization check for the set, the user only need to pass one of the rules 
        /// (OR behavior). However if there are multiple sets in a route, the user will
        /// need to pass all sets (AND behaviour).
        /// </summary>
        public ICollection<EntityAccessRule> AccessRules { get; set; }

        /// <summary>
        /// An action to take when a user does not meet the rule criteria.
        /// </summary>
        public AccessRuleViolationAction ViolationAction { get; set; }

        /// <summary>
        /// This optional field indicates that unathenticated users should be
        /// redirected to the sign in page associated with the specified user 
        /// area. In rare circumstances <see cref="AccessRules"/> may contain
        /// rules for multiple user area, therefore this field helps to 
        /// distinguish which user area should be used for the redirection.
        /// </summary>
        public string UserAreaCodeForSignInRedirect { get; set; }

        /// <summary>
        /// Determines if the <paramref name="user"/> is permitted to
        /// access the resource associated with this ruleset. To pass the
        /// check the user must match any of the <see cref="AccessRules"/>.
        /// If there are no access rules, then the resource is public and 
        /// the check passes.
        /// </summary>
        /// <param name="user">The user to check; cannot be null.</param>
        /// <returns>true if the user is authorized to access this resource; otherwise false.</returns>
        public bool IsAuthorized(IUserContext user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (EnumerableHelper.IsNullOrEmpty(AccessRules))
            {
                return true;
            }

            if (!user.IsSignedIn())
            {
                return false;
            }

            // if logged in, the user should always be assigned a user area
            EntityInvalidOperationException.ThrowIfNull(user, u => u.UserArea);

            return AccessRules
                .Any(r => r.UserAreaCode == user.UserArea.UserAreaCode
                    && (!r.RoleId.HasValue || r.RoleId == user.RoleId));
        }

        /// <summary>
        /// Determines if this rule set requires users that unauthenticated users are
        /// redirected to a sign in page.
        /// </summary>
        public bool ShouldTryRedirect()
        {
            return !string.IsNullOrEmpty(UserAreaCodeForSignInRedirect);
        }
    }
}
