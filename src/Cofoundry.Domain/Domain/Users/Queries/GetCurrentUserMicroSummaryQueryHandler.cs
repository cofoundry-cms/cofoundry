using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
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

        public UserMicroSummary Execute(GetCurrentUserMicroSummaryQuery query, IExecutionContext executionContext)
        {
            if (!executionContext.UserContext.UserId.HasValue) return null;

            var user = _queryExecutor.GetById<UserMicroSummary>(executionContext.UserContext.UserId.Value);
            return user;
        }
    }
}
