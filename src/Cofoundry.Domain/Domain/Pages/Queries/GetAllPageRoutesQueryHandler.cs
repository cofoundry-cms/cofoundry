using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns a collection of page routing data for all pages. The 
    /// PageRoute projection is a small page object focused on providing 
    /// routing data only. Data returned from this query is cached by 
    /// default as it's core to routing and often incorporated in more detailed
    /// page projections.
    /// </summary>
    public class GetAllPageRoutesQueryHandler
        : IQueryHandler<GetAllPageRoutesQuery, ICollection<PageRoute>>
        , IPermissionRestrictedQueryHandler<GetAllPageRoutesQuery, ICollection<PageRoute>>
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetAllPageRoutesQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<ICollection<PageRoute>> ExecuteAsync(GetAllPageRoutesQuery query, IExecutionContext executionContext)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetPageRouteLookupQuery());
            return result.Values;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllPageRoutesQuery query)
        {
            yield return new PageReadPermission();
        }
    }
}
