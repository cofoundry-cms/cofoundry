using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cofoundry.Web.Internal
{
    /// <summary>
    /// Implementation of <see cref="IUserSessionService"/> that uses the ASP.NET
    /// auth mechanisms to persit a login session.
    /// </summary>
    /// <inheritdoc/>
    public class WebUserSessionService : IUserSessionService
    {
        /// <summary>
        /// Auth is cached for the lifetime of a request using an in-memory
        /// based session service, which takes care of some of the management
        /// of multi-user-area implementations for us.
        /// </summary>
        private readonly InMemoryUserSessionService _inMemoryUserSessionService;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IUserContextCache _userContextCache;

        public WebUserSessionService(
            IHttpContextAccessor httpContextAccessor,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IUserContextCache userContextCache
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userContextCache = userContextCache;
            _inMemoryUserSessionService = new InMemoryUserSessionService(_userAreaDefinitionRepository, _userContextCache);
        }

        public int? GetCurrentUserId()
        {
            var cachedUserId = _inMemoryUserSessionService.GetCurrentUserId();
            if (cachedUserId.HasValue) return cachedUserId;

            var user = _httpContextAccessor?.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null) return null;

            // Otherwise get it from the Identity
            var userId = IntParser.ParseOrNull(userIdClaim.Value);
            return userId;
        }

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

            if (userId.HasValue)
            {
                // cache the auth by logging into to the in-memory service
                await _inMemoryUserSessionService.LogUserInAsync(userAreaCode, userId.Value, true);
            }

            return userId;
        }

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

            await _inMemoryUserSessionService.LogUserInAsync(userAreaCode, userId, rememberUser);
        }

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

        public async Task LogUserOutOfAllUserAreasAsync()
        {
            await _inMemoryUserSessionService.LogUserOutOfAllUserAreasAsync();

            foreach (var customEntityDefinition in _userAreaDefinitionRepository.GetAll())
            {
                var scheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(customEntityDefinition.UserAreaCode);
                await _httpContextAccessor.HttpContext.SignOutAsync(scheme);
            }
        }

        public async Task SetAmbientUserAreaAsync(string userAreaCode)
        {
            if (string.IsNullOrWhiteSpace(userAreaCode)) throw new ArgumentEmptyException(nameof(userAreaCode));

            // Ensure that if the user is logged into the area that it is in 
            // the cache before we make the switch. This is because in
            // GetCurrentUserId() we will need to rely on the cache for
            // the UserId
            await GetUserIdByUserAreaCodeAsync(userAreaCode);

            await _inMemoryUserSessionService.SetAmbientUserAreaAsync(userAreaCode);
        }
    }
}