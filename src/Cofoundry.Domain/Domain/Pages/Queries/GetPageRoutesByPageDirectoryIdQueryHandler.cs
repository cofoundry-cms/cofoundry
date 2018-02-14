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
        : IAsyncQueryHandler<GetPageRoutesByPageDirectoryIdQuery, ICollection<PageRoute>>
        , IPermissionRestrictedQueryHandler<GetPageRoutesByPageDirectoryIdQuery, ICollection<PageRoute>>
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetPageRoutesByPageDirectoryIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<ICollection<PageRoute>> ExecuteAsync(GetPageRoutesByPageDirectoryIdQuery query, IExecutionContext executionContext)
        {
            var allPageRoutes = await _queryExecutor.ExecuteAsync(new GetAllPageRoutesQuery(), executionContext);
            var result = allPageRoutes.Where(p => p.PageDirectory.PageDirectoryId == query.PageDirectoryId);

            return result.ToList();
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutesByPageDirectoryIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }

}
