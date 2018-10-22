using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Cofoundry.Web
{
    /// <summary>
    /// Service to abstract away the management of a users current browser session and 
    /// authentication cookie.
    /// </summary>
    public class UserSessionService : IUserSessionService
    {
        /// <summary>
        /// SignInUser doesn't always update the HttpContext.Current.User so we set this
        /// cache value instead which will last for the lifetime of the request. This is only used
        /// when signing in and isn't otherwise cached.
        /// </summary>
        private int? userIdCache = null;
        private string cachedUserIdArea = null;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public UserSessionService(
            IHttpContextAccessor httpContextAccessor,
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
        }

        /// <summary>
        /// Gets the UserId of the user authenticated for the
        /// current request under the ambient authentication scheme.
        /// </summary>
        /// <returns>
        /// Integer UserId or null if the user is not logged in for the ambient
        /// authentication scheme.
        /// </returns>
        public int? GetCurrentUserId()
        {
            if (userIdCache.HasValue) return userIdCache;

            var user = _httpContextAccessor?.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null) return null;

            // Otherwise get it from the Identity
            var userId = IntParser.ParseOrNull(userIdClaim.Value);
            return userId;
        }

        /// <summary>
        /// Gets the UserId of the currently logged in user for a specific UserArea,
        /// regardless of the ambient authentication scheme. Useful in multi-userarea
        /// scenarios where you need to ignore the ambient user and check for permissions 
        /// against a specific user area.
        /// </summary>
        /// <param name="userAreaCode">The unique identifying code fo the user area to check for.</param>
        public async Task<int?> GetUserIdByUserAreaCodeAsync(string userAreaCode)
        {
            if (userAreaCode == null)
            {
                throw new ArgumentNullException(nameof(userAreaCode));
            }

            if (cachedUserIdArea == userAreaCode && userIdCache.HasValue) return userIdCache.Value;

            var scheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(userAreaCode);
            var result = await _httpContextAccessor.HttpContext.AuthenticateAsync(scheme);
            if (!result.Succeeded) return null;

            var userIdClaim = result.Principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;

            var userId = IntParser.ParseOrNull(userIdClaim.Value);
            return userId;
        }

        /// <summary>
        /// Logs the specified UserId into the current session.
        /// </summary>
        /// <param name="userAreaCode">Unique code of the user area to log the user into (required).</param>
        /// <param name="userId">UserId belonging to the owner of the current session.</param>
        /// <param name="rememberUser">
        /// True if the session should last indefinately; false if the 
        /// session should close after a timeout period.
        /// </param>
        public Task LogUserInAsync(string userAreaCode, int userId, bool rememberUser)
        {
            if (userAreaCode == null) throw new ArgumentNullException(nameof(userAreaCode));
            if (userId < 1) throw new ArgumentOutOfRangeException(nameof(userId));

            var stringId = Convert.ToString(userId);
            var scheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(userAreaCode);

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, stringId),
            };

            var claimsIdentity = new ClaimsIdentity(claims, scheme);
            var userPrincipal = new ClaimsPrincipal(claimsIdentity);

            userIdCache = userId;
            cachedUserIdArea = userAreaCode;

            if (rememberUser)
            {
                var authProperties = new AuthenticationProperties() { IsPersistent = true };
                return _httpContextAccessor.HttpContext.SignInAsync(scheme, userPrincipal, authProperties);
            }
            else
            {
                return _httpContextAccessor.HttpContext.SignInAsync(scheme, userPrincipal);
            }
        }

        /// <summary>
        /// Logs the user out of the specified user area.
        /// </summary>
        /// <param name="userAreaCode">Unique code of the user area to log the user out of (required).</param>
        public async Task LogUserOutAsync(string userAreaCode)
        {
            if (userAreaCode == null)
            {
                throw new ArgumentNullException(nameof(userAreaCode));
            }

            if (cachedUserIdArea == userAreaCode)
            {
                ClearCache();
            }

            var scheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(userAreaCode);
            await _httpContextAccessor.HttpContext.SignOutAsync(scheme);
        }

        /// <summary>
        /// Logs the user out of all user areas.
        /// </summary>
        public async Task LogUserOutOfAllUserAreasAsync()
        {
            ClearCache();

            foreach (var customEntityDefinition in _userAreaDefinitionRepository.GetAll())
            {
                var scheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(customEntityDefinition.UserAreaCode);
                await _httpContextAccessor.HttpContext.SignOutAsync(scheme);
            }
        }

        private void ClearCache()
        {
            userIdCache = null;
            cachedUserIdArea = null;
        }
    }
}
