using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a UserMicroSummary object representing the currently logged in 
    /// user. If the user is not logged in then null is returned.
    /// </summary>
    public class GetCurrentUserMicroSummaryQueryHandler 
        : IAsyncQueryHandler<GetCurrentUserMicroSummaryQuery, UserMicroSummary>
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

            var user = _queryExecutor.GetByIdAsync<UserMicroSummary>(executionContext.UserContext.UserId.Value);
            return user;
        }
    }
}
