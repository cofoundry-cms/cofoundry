using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns a page directory with the specified id as a PageDirectoryRoute instance.
    /// </summary>
    public class GetPageDirectoryRouteByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<PageDirectoryRoute>, PageDirectoryRoute>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<PageDirectoryRoute>, PageDirectoryRoute>
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
        
        public async Task<PageDirectoryRoute> ExecuteAsync(GetByIdQuery<PageDirectoryRoute> query, IExecutionContext executionContext)
        {
            var result = (await _queryExecutor
                .GetAllAsync<PageDirectoryRoute>())
                .SingleOrDefault(l => l.PageDirectoryId == query.Id);

            return result;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<PageDirectoryRoute> command)
        {
            yield return new PageDirectoryReadPermission();
        }

        #endregion
    }
}
