using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Cofoundry.Web.Internal;

/// <summary>
/// Implementation of <see cref="IUserSessionService"/> that uses the ASP.NET
/// auth mechanisms to persit a user session.
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

    /// <summary>
    /// HttpContext.User does not clear out the claims principal which causes
    /// issues if the user is requested after sign out has occurred (e.g. in 
    /// view rendering). This is used to track sign-outs and prevent this occuring.
    /// </summary>
    private HashSet<string> _signedOutUserAreas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
    private readonly IUserContextCache _userContextCache;
    private readonly IClaimsPrincipalFactory _claimsPrincipalFactory;
    private readonly IClaimsPrincipalBuilderContextRepository _claimsPrincipalBuilderContextRepository;

    public WebUserSessionService(
        IHttpContextAccessor httpContextAccessor,
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        IUserContextCache userContextCache,
        IClaimsPrincipalFactory claimsPrincipalFactory,
        IClaimsPrincipalBuilderContextRepository claimsPrincipalBuilderContextRepository
        )
    {
        _httpContextAccessor = httpContextAccessor;
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
        _userContextCache = userContextCache;
        _claimsPrincipalFactory = claimsPrincipalFactory;
        _claimsPrincipalBuilderContextRepository = claimsPrincipalBuilderContextRepository;
        _inMemoryUserSessionService = new InMemoryUserSessionService(_userAreaDefinitionRepository, _userContextCache);
    }

    public int? GetCurrentUserId()
    {
        var cachedUserId = _inMemoryUserSessionService.GetCurrentUserId();
        if (cachedUserId.HasValue) return cachedUserId;

        var user = _httpContextAccessor?.HttpContext?.User;
        var userIdClaim = user?.FindFirst(CofoundryClaimTypes.UserId);
        var userAreaClaim = user?.FindFirst(CofoundryClaimTypes.UserAreaCode);

        if (userIdClaim == null || userAreaClaim == null) return null;

        // User has been signed out during this request so ignore
        if (_signedOutUserAreas.Contains(userAreaClaim.Value)) return null;

        // Otherwise get it from the Identity
        var userId = IntParser.ParseOrNull(userIdClaim.Value);
        return userId;
    }

    public async Task<int?> GetUserIdByUserAreaCodeAsync(string userAreaCode)
    {
        ArgumentNullException.ThrowIfNull(userAreaCode);

        if (_signedOutUserAreas.Contains(userAreaCode)) return null;
        var cachedUserId = await _inMemoryUserSessionService.GetUserIdByUserAreaCodeAsync(userAreaCode);
        if (cachedUserId.HasValue) return cachedUserId;

        if (_httpContextAccessor.HttpContext == null) return null;

        var scheme = AuthenticationSchemeNames.UserArea(userAreaCode);
        var result = await _httpContextAccessor.HttpContext.AuthenticateAsync(scheme);
        if (!result.Succeeded) return null;

        var userIdClaim = result.Principal.FindFirst(CofoundryClaimTypes.UserId);
        if (userIdClaim == null) return null;

        var userId = IntParser.ParseOrNull(userIdClaim.Value);

        if (userId.HasValue)
        {
            // cache the auth by logging into the in-memory service
            await _inMemoryUserSessionService.SignInAsync(userAreaCode, userId.Value, true);
        }

        return userId;
    }

    public async Task SignInAsync(string userAreaCode, int userId, bool rememberUser)
    {
        ArgumentNullException.ThrowIfNull(userAreaCode);
        if (userId < 1) throw new ArgumentOutOfRangeException(nameof(userId));
        ValidateHttpContext("sign in a user");

        var userArea = _userAreaDefinitionRepository.GetRequiredByCode(userAreaCode);
        var userPrincipal = await CreateUserPrincipal(userId, userArea);
        var scheme = AuthenticationSchemeNames.UserArea(userArea.UserAreaCode);

        if (rememberUser)
        {
            var authProperties = new AuthenticationProperties() { IsPersistent = true };
            await _httpContextAccessor.HttpContext.SignInAsync(scheme, userPrincipal, authProperties);
        }
        else
        {
            await _httpContextAccessor.HttpContext.SignInAsync(scheme, userPrincipal);
        }

        await _inMemoryUserSessionService.SignInAsync(userAreaCode, userId, rememberUser);
        if (_signedOutUserAreas.Contains(userAreaCode))
        {
            // signed out and back in during the same request: odd but let's handle it.
            _signedOutUserAreas.Remove(userAreaCode);
        }
    }

    public async Task SignOutAsync(string userAreaCode)
    {
        ArgumentNullException.ThrowIfNull(userAreaCode);
        ValidateHttpContext("sign out a user");

        await _inMemoryUserSessionService.SignOutAsync(userAreaCode);

        var scheme = AuthenticationSchemeNames.UserArea(userAreaCode);
        await _httpContextAccessor.HttpContext.SignOutAsync(scheme);
        TrackSignOut(userAreaCode);
    }

    public async Task SignOutOfAllUserAreasAsync()
    {
        ValidateHttpContext("sign out a user");
        await _inMemoryUserSessionService.SignOutOfAllUserAreasAsync();

        foreach (var userArea in _userAreaDefinitionRepository.GetAll())
        {
            var scheme = AuthenticationSchemeNames.UserArea(userArea.UserAreaCode);
            await _httpContextAccessor.HttpContext.SignOutAsync(scheme);
            TrackSignOut(userArea.UserAreaCode);
        }
    }

    public async Task SetAmbientUserAreaAsync(string userAreaCode)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(userAreaCode);

        // Ensure that if the user is logged into the area that it is in 
        // the cache before we make the switch. This is because in
        // GetCurrentUserId() we will need to rely on the cache for
        // the UserId
        await GetUserIdByUserAreaCodeAsync(userAreaCode);

        await _inMemoryUserSessionService.SetAmbientUserAreaAsync(userAreaCode);
    }

    public async Task RefreshAsync(string userAreaCode, int userId)
    {
        ArgumentNullException.ThrowIfNull(userAreaCode);
        if (userId < 1) throw new ArgumentOutOfRangeException(nameof(userId));
        ValidateHttpContext("refresh a user sign in");

        var userArea = _userAreaDefinitionRepository.GetRequiredByCode(userAreaCode);
        var loggedInUser = await GetUserIdByUserAreaCodeAsync(userAreaCode);

        // Only refresh the sign in if the user is currently logged in
        if (loggedInUser != userId) return;

        var scheme = AuthenticationSchemeNames.UserArea(userArea.UserAreaCode);

        var auth = await _httpContextAccessor.HttpContext.AuthenticateAsync(scheme);
        var isPeristent = auth?.Properties?.IsPersistent ?? false;

        await SignInAsync(userAreaCode, userId, isPeristent);
    }

    private void ValidateHttpContext(string actionDescription)
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            throw new InvalidOperationException($"{nameof(_httpContextAccessor)}.HttpContext is null. Cannot use {nameof(WebUserSessionService)} to {actionDescription} outside of a web request.");
        }
    }

    private async Task<ClaimsPrincipal> CreateUserPrincipal(int userId, IUserAreaDefinition userArea)
    {
        var context = await _claimsPrincipalBuilderContextRepository.GetAsync(userId);
        EntityNotFoundException.ThrowIfNull(context, userId);

        if (context.UserAreaCode != userArea.UserAreaCode)
        {
            throw new InvalidOperationException($"Cannot log user {userId} into user area {userArea.UserAreaCode} because they belong to user area {context.UserAreaCode}");
        }

        var userPrincipal = await _claimsPrincipalFactory.CreateAsync(context);
        if (userPrincipal == null)
        {
            throw new InvalidOperationException($"{_claimsPrincipalFactory.GetType().Name}.{nameof(_claimsPrincipalFactory.CreateAsync)} did not return a value.");
        }

        return userPrincipal;
    }

    private void TrackSignOut(string userAreaCode)
    {
        if (!_signedOutUserAreas.Contains(userAreaCode))
        {
            _signedOutUserAreas.Add(userAreaCode);
        }
    }
}
