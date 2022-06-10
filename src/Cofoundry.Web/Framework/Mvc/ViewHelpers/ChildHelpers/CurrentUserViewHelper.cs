namespace Cofoundry.Web.Internal;

/// <summary>
/// A view helper for providing information about the currently logged in user
/// </summary>
public class CurrentUserViewHelper : ICurrentUserViewHelper
{
    private ICurrentUserViewHelperContext _helperContext = null;
    private Dictionary<string, ICurrentUserViewHelperContext> _alternativeHelperContextCache = new Dictionary<string, ICurrentUserViewHelperContext>();

    private readonly IUserContextService _userContextService;
    private readonly IQueryExecutor _queryExecutor;

    public CurrentUserViewHelper(
        IUserContextService userContextServiceService,
        IQueryExecutor queryExecutor
        )
    {
        _userContextService = userContextServiceService;
        _queryExecutor = queryExecutor;
    }

    /// <summary>
    /// Returns information about the currently logged in user. If your 
    /// project has multiple user areas then this method will run on the
    /// user area marked as the default auth schema. Once the user data is 
    /// loaded it is cached so you don't have to worry about calling this 
    /// multiple times.
    /// </summary>
    public async Task<ICurrentUserViewHelperContext> GetAsync()
    {
        // since this only runs in views it shouldn't need to be threadsafe
        if (_helperContext == null)
        {
            var userContext = await _userContextService.GetCurrentContextAsync();
            _helperContext = await GetHelperContextAsync(userContext);
        }

        return _helperContext;
    }

    /// <summary>
    /// Returns information about the currently logged in user for a 
    /// specific user area. This is useful if you have multiple user
    /// areas because only one can be set as the default auth schema.
    /// Once the user data is loaded it is cached so you don't have to worry 
    /// about calling this multiple times.
    /// </summary>
    /// <param name="userAreaCode">
    /// The unique 3 letter identifier code for the user area to check for.
    /// </param>
    public async Task<ICurrentUserViewHelperContext> GetAsync(string userAreaCode)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(userAreaCode);

        if (_helperContext?.Data?.UserArea?.UserAreaCode == userAreaCode)
        {
            return _helperContext;
        }

        var helperContext = _alternativeHelperContextCache.GetOrDefault(userAreaCode);

        if (helperContext != null)
        {
            return helperContext;
        }

        var userContext = await _userContextService.GetCurrentContextByUserAreaAsync(userAreaCode);
        helperContext = await GetHelperContextAsync(userContext);

        _alternativeHelperContextCache.Add(userAreaCode, helperContext);

        return helperContext;
    }

    private async Task<ICurrentUserViewHelperContext> GetHelperContextAsync(IUserContext userContext)
    {
        var context = new CurrentUserViewHelperContext();
        context.Role = await _queryExecutor.ExecuteAsync(new GetRoleDetailsByIdQuery(userContext.RoleId), userContext);

        if (userContext.UserId.HasValue)
        {
            var query = new GetUserSummaryByIdQuery(userContext.UserId.Value);
            context.Data = await _queryExecutor.ExecuteAsync(query, userContext);
            context.IsSignedIn = true;
        }

        return context;
    }
}
