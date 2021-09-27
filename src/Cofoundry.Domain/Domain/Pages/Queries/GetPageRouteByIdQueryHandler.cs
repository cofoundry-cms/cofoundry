using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns page routing data for a single page. The 
    /// PageRoute projection is a small page object focused on providing 
    /// routing data only. Data returned from this query is cached by 
    /// default as it's core to routing and often incorporated in more detailed
    /// page projections.
    /// </summary>
    public class GetPageRouteByIdQueryHandler
        : IQueryHandler<GetPageRouteByIdQuery, PageRoute>
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
            var allPageRoutes = await _queryExecutor.ExecuteAsync(new GetPageRouteLookupQuery(), executionContext);
            var result = allPageRoutes.GetOrDefault(query.PageId);

            return result;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRouteByIdQuery query)
        {
            yield return new PageReadPermission();
        }
    }
}
