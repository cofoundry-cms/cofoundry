using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns a page directory with the specified id as a PageDirectoryRoute instance.
    /// </summary>
    public class GetPageDirectoryRouteByIdQueryHandler 
        : IQueryHandler<GetPageDirectoryRouteByIdQuery, PageDirectoryRoute>
        , IPermissionRestrictedQueryHandler<GetPageDirectoryRouteByIdQuery, PageDirectoryRoute>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public GetPageDirectoryRouteByIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution
        
        public async Task<PageDirectoryRoute> ExecuteAsync(GetPageDirectoryRouteByIdQuery query, IExecutionContext executionContext)
        {
            var allRoutes = await _queryExecutor.ExecuteAsync(new GetAllPageDirectoryRoutesQuery(), executionContext);
            var result = allRoutes.SingleOrDefault(l => l.PageDirectoryId == query.PageDirectoryId);

            return result;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageDirectoryRouteByIdQuery command)
        {
            yield return new PageDirectoryReadPermission();
        }

        #endregion
    }
}
