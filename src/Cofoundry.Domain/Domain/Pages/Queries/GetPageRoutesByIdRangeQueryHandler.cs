using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public class GetPageRoutesByIdRangeQueryHandler 
        : IQueryHandler<GetPageRoutesByIdRangeQuery, IDictionary<int, PageRoute>>
        , IPermissionRestrictedQueryHandler<GetPageRoutesByIdRangeQuery, IDictionary<int, PageRoute>>
    {
        private readonly IQueryExecutor _queryExecutor;
         
        public GetPageRoutesByIdRangeQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<IDictionary<int, PageRoute>> ExecuteAsync(GetPageRoutesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var allPageRoutes = await _queryExecutor.ExecuteAsync(new GetPageRouteLookupQuery(), executionContext);
            var result = allPageRoutes
                .FilterByKeys(query.PageIds)
                .ToDictionary(r => r.PageId);

            return result;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutesByIdRangeQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
