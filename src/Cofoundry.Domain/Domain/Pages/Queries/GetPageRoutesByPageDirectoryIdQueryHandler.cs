using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns page routing data for pages that are nested immediately inside the specified 
    /// directory. The PageRoute projection is a small page object focused on providing 
    /// routing data only. Data returned from this query is cached by 
    /// default as it's core to routing and often incorporated in more detailed
    /// page projections.
    /// </summary>
    public class GetPageRoutesByPageDirectoryIdQueryHandler 
        : IQueryHandler<GetPageRoutesByPageDirectoryIdQuery, ICollection<PageRoute>>
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
