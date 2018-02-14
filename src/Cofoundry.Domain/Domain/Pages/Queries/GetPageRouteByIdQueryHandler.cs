using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRouteByIdQueryHandler 
        : IAsyncQueryHandler<GetPageRouteByIdQuery, PageRoute>
        , IPermissionRestrictedQueryHandler<GetPageRouteByIdQuery, PageRoute>
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetPageRouteByIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<PageRoute> ExecuteAsync(GetPageRouteByIdQuery query, IExecutionContext executionContext)
        {
            var allPageRoutes = await _queryExecutor.ExecuteAsync(new GetAllPageRoutesQuery(), executionContext);
            var result = allPageRoutes.SingleOrDefault(p => p.PageId == query.PageId);

            return result;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRouteByIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
