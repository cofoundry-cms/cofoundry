using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRoutesByIdRangeQueryHandler 
        : IQueryHandler<GetByIdRangeQuery<PageRoute>, IDictionary<int, PageRoute>>
        , IAsyncQueryHandler<GetByIdRangeQuery<PageRoute>, IDictionary<int, PageRoute>>
        , IPermissionRestrictedQueryHandler<GetByIdRangeQuery<PageRoute>, IDictionary<int, PageRoute>>
    {
        private readonly IQueryExecutor _queryExecutor;
         
        public GetPageRoutesByIdRangeQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public IDictionary<int, PageRoute> Execute(GetByIdRangeQuery<PageRoute> query, IExecutionContext executionContext)
        {
            var result = _queryExecutor
                .GetAll<PageRoute>(executionContext)
                .Where(r => query.Ids.Contains(r.PageId))
                .ToDictionary(r => r.PageId);

            return result;
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
