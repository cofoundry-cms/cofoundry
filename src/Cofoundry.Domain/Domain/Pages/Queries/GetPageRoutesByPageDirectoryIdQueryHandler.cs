using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRoutesByPageDirectoryIdQueryHandler 
        : IAsyncQueryHandler<GetPageRoutesByPageDirectoryIdQuery, IEnumerable<PageRoute>>
        , IPermissionRestrictedQueryHandler<GetPageRoutesByPageDirectoryIdQuery, IEnumerable<PageRoute>>
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetPageRoutesByPageDirectoryIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<IEnumerable<PageRoute>> ExecuteAsync(GetPageRoutesByPageDirectoryIdQuery query, IExecutionContext executionContext)
        {
            var allRoutes = await _queryExecutor.GetAllAsync<PageRoute>(executionContext);
            var result = allRoutes.Where(p => p.PageDirectory.PageDirectoryId == query.PageDirectoryId);

            return result;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutesByPageDirectoryIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }

}
