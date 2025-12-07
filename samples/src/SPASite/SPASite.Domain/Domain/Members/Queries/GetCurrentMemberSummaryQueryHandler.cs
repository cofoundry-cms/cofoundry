namespace SPASite.Domain;

/// <summary>
/// Query to get some information about the currently logged in user. We can use
/// <see cref="IIgnorePermissionCheckHandler"/> here because if the user is not
/// logged in then we return null, so there's no need for a permission check.
/// </summary>
public class GetCurrentMemberSummaryQueryHandler
    : IQueryHandler<GetCurrentMemberSummaryQuery, MemberSummary?>
    , IIgnorePermissionCheckHandler
{
    private readonly IContentRepository _contentRepository;

    public GetCurrentMemberSummaryQueryHandler(
        IContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    public async Task<MemberSummary?> ExecuteAsync(GetCurrentMemberSummaryQuery query, IExecutionContext executionContext)
    {
        var userContext = executionContext.UserContext.ToSignedInContext();
        if (userContext == null || userContext.UserArea.UserAreaCode != MemberUserArea.Code)
        {
            return null;
        }

        var user = await _contentRepository
            .Users()
            .Current()
            .Get()
            .AsMicroSummary()
            .ExecuteAsync();

        if (user == null)
        {
            return null;
        }

        return new MemberSummary()
        {
            UserId = user.UserId,
            DisplayName = user.DisplayName ?? "Unknown"
        };
    }
}
