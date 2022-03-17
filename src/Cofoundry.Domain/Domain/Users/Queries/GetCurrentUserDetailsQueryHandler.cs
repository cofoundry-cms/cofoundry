namespace Cofoundry.Domain.Internal;

/// <summary>
/// Gets a <see cref="UserSummary"/> object representing the currently logged in  user. If 
/// the user is not logged in then <see langword="null"/> is returned. If  multiple user areas 
/// are implemented, then the returned user will depend on the "ambient" auth scheme, which 
/// is typically the default user area unless the ambient scheme has been changed during 
/// the flow of the request e.g. via an AuthorizeUserAreaAttribute.
/// </summary>
public class GetCurrentUserDetailsQueryHandler
    : IQueryHandler<GetCurrentUserDetailsQuery, UserDetails>
    , IIgnorePermissionCheckHandler
{
    private readonly IDomainRepository _domainRepository;
    private readonly IUserContextService _userContextService;

    public GetCurrentUserDetailsQueryHandler(
        IDomainRepository domainRepository,
        IUserContextService userContextService
        )
    {
        _domainRepository = domainRepository;
        _userContextService = userContextService;
    }

    public async Task<UserDetails> ExecuteAsync(GetCurrentUserDetailsQuery query, IExecutionContext executionContext)
    {
        var userId = executionContext.UserContext.UserId;
        if (!userId.HasValue) return null;

        var user = await _domainRepository
            .WithContext(executionContext)
            .ExecuteQueryAsync(new GetUserDetailsByIdQuery(userId.Value));

        return user;
    }
}
