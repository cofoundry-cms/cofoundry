using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
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
        private readonly IUserAreaRepository _userAreaRepository;

        public UserSessionService(
            IHttpContextAccessor httpContextAccessor,
            IUserAreaRepository userAreaRepository
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _userAreaRepository = userAreaRepository;
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

            //if (user == null || !user.Identity.IsAuthenticated) return null;

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
        public Task SetCurrentUserIdAsync(string userAreaDefinitionCode, int userId, bool rememberUser)
        {
            if (userAreaDefinitionCode == null) throw new ArgumentNullException(nameof(userAreaDefinitionCode));
            if (userId < 1) throw new ArgumentOutOfRangeException(nameof(userId));

            var stringId = Convert.ToString(userId);

            var userPrincipal = new GenericPrincipal(new GenericIdentity(stringId), null);
            userIdCache = userId;

            var scheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(userAreaDefinitionCode);
            return _httpContextAccessor.HttpContext.Authentication.SignInAsync(scheme, userPrincipal, _defaultAuthenticationProperties);
        }

        /// <summary>
        /// Abandons the current session and removes the users
        /// login cookie
        /// </summary>
        public async Task AbandonAsync()
        {
            if (_httpContextAccessor.HttpContext?.User == null) return;

            userIdCache = null;
            //_httpContextAccessor.HttpContext?.Session?.Clear();

            foreach (var customEntityDefinition in _userAreaRepository.GetAll())
            {
                var scheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(customEntityDefinition.UserAreaCode);
                await _httpContextAccessor.HttpContext.Authentication.SignOutAsync(scheme);
            }

        }
    }
}
