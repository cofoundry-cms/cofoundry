using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRoutesByWebDirectoryIdQueryHandler 
        : IAsyncQueryHandler<GetPageRoutesByWebDirectoryIdQuery, IEnumerable<PageRoute>>
        , IPermissionRestrictedQueryHandler<GetPageRoutesByWebDirectoryIdQuery, IEnumerable<PageRoute>>
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetPageRoutesByWebDirectoryIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<IEnumerable<PageRoute>> ExecuteAsync(GetPageRoutesByWebDirectoryIdQuery query, IExecutionContext executionContext)
        {
            var allRoutes = await _queryExecutor.GetAllAsync<PageRoute>(executionContext);
            var result = allRoutes.Where(p => p.WebDirectory.WebDirectoryId == query.WebDirectoryId);

            return result;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutesByWebDirectoryIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }

}
