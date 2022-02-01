using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Gets a <see cref="UserMicroSummary"/> object representing the currently logged in 
    /// user. If the user is not logged in then <see langword="null"/> is returned. If  multiple user areas 
    /// are implemented, then the returned user will depend on the "ambient" auth scheme, which 
    /// is typically the default user area unless the ambient scheme has been changed during 
    /// the flow of the request e.g. via an AuthorizeUserAreaAttribute.
    /// </summary>
    public class GetCurrentUserMicroSummaryQueryHandler
        : IQueryHandler<GetCurrentUserMicroSummaryQuery, UserMicroSummary>
        , IIgnorePermissionCheckHandler
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IUserContextService _userContextService;

        public GetCurrentUserMicroSummaryQueryHandler(
            IDomainRepository domainRepository,
            IUserContextService userContextService
            )
        {
            _domainRepository = domainRepository;
            _userContextService = userContextService;
        }

        public async Task<UserMicroSummary> ExecuteAsync(GetCurrentUserMicroSummaryQuery query, IExecutionContext executionContext)
        {
            var userId = executionContext.UserContext.UserId;
            if (!userId.HasValue) return null;

            var user = await _domainRepository
                .WithContext(executionContext)
                .ExecuteQueryAsync(new GetUserMicroSummaryByIdQuery(userId.Value));

            return user;
        }
    }
}
