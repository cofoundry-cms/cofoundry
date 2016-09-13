using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRouteByIdRangeQueryHandler 
        : IAsyncQueryHandler<GetByIdRangeQuery<PageRoute>, IDictionary<int, PageRoute>>
        , IPermissionRestrictedQueryHandler<GetByIdRangeQuery<PageRoute>, IDictionary<int, PageRoute>>
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetPageRouteByIdRangeQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<IDictionary<int, PageRoute>> ExecuteAsync(GetByIdRangeQuery<PageRoute> query, IExecutionContext executionContext)
        {
            var result = (await _queryExecutor
                .GetAllAsync<PageRoute>(executionContext))
                .Where(r => query.Ids.Contains(r.PageId))
                .ToDictionary(r => r.PageId);

            return result;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdRangeQuery<PageRoute> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
