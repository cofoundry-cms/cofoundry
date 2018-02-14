using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRoutesByIdRangeQueryHandler 
        : IAsyncQueryHandler<GetPageRoutesByIdRangeQuery, IDictionary<int, PageRoute>>
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
            var allPageRoutes = await _queryExecutor.ExecuteAsync(new GetAllPageRoutesQuery(), executionContext);
            var result = allPageRoutes
                .Where(r => query.PageIds.Contains(r.PageId))
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
