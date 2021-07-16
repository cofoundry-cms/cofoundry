using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Web.Internal
{
    /// <summary>
    /// Service to abstract away the management of a users current browser session and 
    /// authentication cookie.
    /// </summary>
    public class WebUserSessionService : IUserSessionService
    {
        /// <summary>
        /// SignInUser doesn't always update the HttpContext.Current.User so we use an in-memory
        /// cache instead which will last for the lifetime of the request. This is only used
        /// when signing in and isn't otherwise cached.
        /// </summary>
        private readonly InMemoryUserSessionService _inMemoryUserSessionService;
        private string _ambientCachedSchemaName = null;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public WebUserSessionService(
            IHttpContextAccessor httpContextAccessor,
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _inMemoryUserSessionService = new InMemoryUserSessionService(_userAreaDefinitionRepository);
        }

        /// <summary>
        /// Gets the UserId of the user authenticated for the current request under 
        /// the ambient authentication scheme. The ambient scheme could be the default
        /// or one that is specified using an AuthorizeAttribute.
        /// </summary>
        /// <returns>
        /// Integer UserId or null if the user is not logged in for the ambient
        /// authentication scheme.
        /// </returns>
        public int? GetCurrentUserId()
        {
            if (_ambientCachedSchemaName != null)
            {
                var cachedUserId = _inMemoryUserSessionService.GetUserIdByUserAreaCode(_ambientCachedSchemaName);
                if (cachedUserId.HasValue) return cachedUserId;
            }

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

            var cachedUserId = await _inMemoryUserSessionService.GetUserIdByUserAreaCodeAsync(userAreaCode);
            if (cachedUserId.HasValue) return cachedUserId;

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
        public async Task LogUserInAsync(string userAreaCode, int userId, bool rememberUser)
        {
            if (userAreaCode == null) throw new ArgumentNullException(nameof(userAreaCode));
            if (userId < 1) throw new ArgumentOutOfRangeException(nameof(userId));

            var userArea = _userAreaDefinitionRepository.GetByCode(userAreaCode);
            EntityNotFoundException.ThrowIfNull(userArea, userAreaCode);

            var scheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(userArea.UserAreaCode);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Convert.ToString(userId))
            };

            var claimsIdentity = new ClaimsIdentity(claims, scheme);
            var userPrincipal = new ClaimsPrincipal(claimsIdentity);

            if (rememberUser)
            {
                var authProperties = new AuthenticationProperties() { IsPersistent = true };
                await _httpContextAccessor.HttpContext.SignInAsync(scheme, userPrincipal, authProperties);
            }
            else
            {
                await _httpContextAccessor.HttpContext.SignInAsync(scheme, userPrincipal);
            }

            await CacheLoginForRequestLifetime(userId, rememberUser, userArea);
        }

        /// <summary>
        /// We need to cache the userId value in memory for this request because it won't be retrievable 
        /// from HttpContext. It's rare that this is actually required as typically a request will end
        /// after the user is logged in.
        /// </summary>
        private async Task CacheLoginForRequestLifetime(int userId, bool rememberUser, IUserAreaDefinition userArea)
        {
            await _inMemoryUserSessionService.LogUserInAsync(userArea.UserAreaCode, userId, rememberUser);
            if (_ambientCachedSchemaName == null)
            {
                // We're not able to tell what the ambient schema applied to the controller (or similar) is, 
                // so we'll have to assume that the first login in schema is ambient schema
                // There's no real use-case yet for multiple logins during the same request so this shouldn't
                // be a problem.
                _ambientCachedSchemaName = userArea.UserAreaCode;
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

            await _inMemoryUserSessionService.LogUserOutAsync(userAreaCode);

            var scheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(userAreaCode);
            await _httpContextAccessor.HttpContext.SignOutAsync(scheme);
        }

        /// <summary>
        /// Logs the user out of all user areas.
        /// </summary>
        public async Task LogUserOutOfAllUserAreasAsync()
        {
            await _inMemoryUserSessionService.LogUserOutOfAllUserAreasAsync();

            foreach (var customEntityDefinition in _userAreaDefinitionRepository.GetAll())
            {
                var scheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(customEntityDefinition.UserAreaCode);
                await _httpContextAccessor.HttpContext.SignOutAsync(scheme);
            }
        }
    }
}
