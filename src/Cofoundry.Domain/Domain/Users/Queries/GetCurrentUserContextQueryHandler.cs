using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// <para>
    /// Gets basic information about the currently logged in user. If the user is not 
    /// logged in then <see cref="UserContext.Empty"/> is returned. If multiple user
    /// areas are implemented, then the returned user will depend on the "ambient" 
    /// auth scheme, which is typically the default user area unless the ambient scheme 
    /// has been changed during the flow of the request e.g. via an AuthorizeUserAreaAttribute.
    /// </para>
    /// <para>
    /// By default the <see cref="IUserContext"/> is cached for the lifetime of the service 
    /// (per request in web apps).
    /// </para>
    /// </summary>
    public class GetCurrentUserContextQueryHandler
        : IQueryHandler<GetCurrentUserContextQuery, IUserContext>
        , IIgnorePermissionCheckHandler
    {
        private readonly IUserContextService _userContextService;

        public GetCurrentUserContextQueryHandler(
            IUserContextService userContextService
            )
        {
            _userContextService = userContextService;
        }

        public async Task<IUserContext> ExecuteAsync(GetCurrentUserContextQuery query, IExecutionContext executionContext)
        {
            if (string.IsNullOrEmpty(query.UserAreaCode))
            {
                return executionContext.UserContext;
            }

            var user = await _userContextService.GetCurrentContextByUserAreaAsync(query.UserAreaCode);

            return user;
        }
    }
}