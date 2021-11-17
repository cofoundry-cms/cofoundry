using Cofoundry.Core;

namespace Cofoundry.Web
{
    /// <summary>
    /// Used to construct or reference the authentication schemes
    /// defined by Cofoundry.
    /// </summary>
    public static class AuthenticationSchemeNames
    {
        /// <summary>
        /// Formats the authentication scheme name used by Cofoundry to configure
        /// the authentication for the specified user area. By default each user area
        /// is configured to use a different scheme configured to use cookie authentication.
        /// </summary>
        /// <param name="userAreaCode">The user area code identifier to format a scheme name for.</param>
        /// <returns>Namespaced scheme name in the format 'Cofoundry_UserArea_Scheme_{userAreaCode}'.</returns>
        public static string UserArea(string userAreaCode)
        {
            if (string.IsNullOrWhiteSpace(userAreaCode)) throw new ArgumentEmptyException(nameof(userAreaCode));

            return "Cofoundry_UserArea_Scheme_" + userAreaCode;
        }
    }
}
