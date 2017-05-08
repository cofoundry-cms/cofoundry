using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;

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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly AuthenticationProperties _defaultAuthenticationProperties = new AuthenticationProperties() { IsPersistent = true };

        public UserSessionService(
            IHttpContextAccessor httpContextAccessor
            )
        {
            _httpContextAccessor = httpContextAccessor;
        }

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

            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated) return null;

            // Otherwise get it from the Identity
            var userId = IntParser.ParseOrNull(user.Identity.Name);
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
        public Task SetCurrentUserIdAsync(int userId, bool rememberUser)
        {
            Condition.Requires(userId).IsGreaterThan(0);
            var stringId = Convert.ToString(userId);

            var userPrincipal = new GenericPrincipal(new GenericIdentity(stringId), null);
            userIdCache = userId;

            return _httpContextAccessor.HttpContext.Authentication.SignInAsync(CofoundryAuthenticationConstants.CookieAuthenticationScheme, userPrincipal, _defaultAuthenticationProperties);
        }

        /// <summary>
        /// Abandons the current session and removes the users
        /// login cookie
        /// </summary>
        public Task AbandonAsync()
        {
            if (_httpContextAccessor.HttpContext?.User == null) return Task.CompletedTask;

            userIdCache = null;
            _httpContextAccessor.HttpContext?.Session?.Clear();

            return _httpContextAccessor.HttpContext.Authentication.SignOutAsync(CofoundryAuthenticationConstants.CookieAuthenticationScheme);
        }
    }
}
