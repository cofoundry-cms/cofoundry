using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Gets a UserDetails object representing the currently logged in 
    /// user. If the user is not logged in then null is returned.
    /// </summary>
    public class GetCurrentUserDetailsQueryHandler
        : IQueryHandler<GetCurrentUserDetailsQuery, UserDetails>
        , IIgnorePermissionCheckHandler
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetCurrentUserDetailsQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public Task<UserDetails> ExecuteAsync(GetCurrentUserDetailsQuery query, IExecutionContext executionContext)
        {
            if (!executionContext.UserContext.UserId.HasValue) return null;

            var userQuery = new GetUserDetailsByIdQuery(executionContext.UserContext.UserId.Value);

            return _queryExecutor.ExecuteAsync(userQuery, executionContext);
        }
    }
}
