using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain;
using System.Security.Principal;

namespace Cofoundry.Web
{
    /// <summary>
    /// Service to abstract away the management of a users current browser session and 
    /// authentication cookie.
    /// </summary>
    public class UserSessionService : IUserSessionService
    {
        /// <summary>
        /// SetAuthCookie doesn't update the HttpContext.Current.User.Identity so we set this
        /// cache value instead which will last for the lifetime of the request. 
        /// </summary>
        private int? userIdCache = null;

        /// <summary>
        /// Gets the UserId of the user currently logged
        /// in to this session
        /// </summary>
        /// <returns>
        /// UserId of the user currently logged
        /// in to this session
        /// </returns>
        public int? GetCurrentUserId()
        {
            // Use the cache if it has been set
            if (userIdCache.HasValue) return userIdCache;

            if (HttpContext.Current == null || HttpContext.Current.User == null || !HttpContext.Current.User.Identity.IsAuthenticated) return null;
            // Otherwise get it from the Identity
            var userId = IntParser.ParseOrNull(HttpContext.Current.User.Identity.Name);
            return userId;
        }

        /// <summary>
        /// Assigns the specified UserId to the current session.
        /// </summary>
        /// <param name="userId">UserId belonging to the owner of the current session.</param>
        /// <param name="rememberUser">
        /// True if the session should last indefinately; false if the 
        /// session should close after a timeout period.
        /// </param>
        public void SetCurrentUserId(int userId, bool rememberUser)
        {
            Condition.Requires(userId).IsGreaterThan(0);
            var stringId = Convert.ToString(userId);
            FormsAuthentication.SetAuthCookie(stringId, rememberUser);
            // Need to manually set the user here so it is available in the rest of the request
            // http://stackoverflow.com/questions/7233939/mvc3-antiforgerytoken-issue
            HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(stringId), null);
            userIdCache = userId;
        }

        /// <summary>
        /// Abandons the current session and removes the users
        /// login cookie
        /// </summary>
        public void Abandon()
        {
            if (HttpContext.Current == null || HttpContext.Current.User == null) return;

            userIdCache = null;
            FormsAuthentication.SignOut();
            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Abandon();
            }
        }
    }
}
