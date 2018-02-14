using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRoutingInfoByCustomEntityIdQueryHandler 
        : IAsyncQueryHandler<GetPageRoutingInfoByCustomEntityIdQuery, ICollection<PageRoutingInfo>>
        , IPermissionRestrictedQueryHandler<GetPageRoutingInfoByCustomEntityIdQuery, ICollection<PageRoutingInfo>>
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetPageRoutingInfoByCustomEntityIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<ICollection<PageRoutingInfo>> ExecuteAsync(GetPageRoutingInfoByCustomEntityIdQuery query, IExecutionContext executionContext)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetPageRoutingInfoByCustomEntityIdRangeQuery(new int[] { query.CustomEntityId }), executionContext);

            if (!result.ContainsKey(query.CustomEntityId)) return Array.Empty<PageRoutingInfo>();

            return result[query.CustomEntityId]
                .OrderBy(e => e.PageRoute.UrlPath.Length)
                .ToList();
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutingInfoByCustomEntityIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }

}
