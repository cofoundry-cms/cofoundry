using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Gets a UserMicroSummary object representing the currently logged in 
    /// user. If the user is not logged in then null is returned.
    /// </summary>
    public class GetCurrentUserMicroSummaryQueryHandler 
        : IQueryHandler<GetCurrentUserMicroSummaryQuery, UserMicroSummary>
        , IIgnorePermissionCheckHandler
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetCurrentUserMicroSummaryQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public Task<UserMicroSummary> ExecuteAsync(GetCurrentUserMicroSummaryQuery query, IExecutionContext executionContext)
        {
            if (!executionContext.UserContext.UserId.HasValue) return null;

            var userQuery = new GetUserMicroSummaryByIdQuery(executionContext.UserContext.UserId.Value);

            return _queryExecutor.ExecuteAsync(userQuery, executionContext);
        }
    }
}
