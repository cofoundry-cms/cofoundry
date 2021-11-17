using Cofoundry.Core;

namespace Cofoundry.Web
{
    /// <summary>
    /// Used to construct or reference the names of authorization policies
    /// defined by Cofoundry.
    /// </summary>
    public static class AuthorizationPolicyNames
    {
        /// <summary>
        /// Formats the name of a policy that restricts authorization to
        /// a specific user area.
        /// </summary>
        /// <param name="userAreaCode">The unique 3 character identifier for the user area to restrict access to.</param>
        /// <returns>Namespaced policy name in the format 'Cofoundry_UserArea_{userAreaCode}'</returns>
        public static string UserArea(string userAreaCode)
        {
            if (string.IsNullOrWhiteSpace(userAreaCode)) throw new ArgumentEmptyException(nameof(userAreaCode));

            return $"Cofoundry_UserArea_{userAreaCode}";
        }

        /// <summary>
        /// Formats the name of a policy that restricts authorization to
        /// a specific Cofoundry role.
        /// </summary>
        /// <param name="userAreaCode">
        /// The unique 3 charcter code identifier for the user area that the role belongs to.
        /// </param>
        /// <param name="roleCode">
        /// The unique 3 character identifier for the role that the requirement 
        /// should authorize.
        /// </param>
        /// <returns>Namespaced policy name in the format 'Cofoundry_UserArea_{userAreaCode}_Role_{roleCode}'</returns>
        public static string Role(string userAreaCode, string roleCode)
        {
            if (string.IsNullOrWhiteSpace(userAreaCode)) throw new ArgumentEmptyException(nameof(userAreaCode));
            if (string.IsNullOrWhiteSpace(roleCode)) throw new ArgumentEmptyException(nameof(roleCode));

            return UserArea(userAreaCode) + $"_Role_{roleCode}";
        }
    }
}
