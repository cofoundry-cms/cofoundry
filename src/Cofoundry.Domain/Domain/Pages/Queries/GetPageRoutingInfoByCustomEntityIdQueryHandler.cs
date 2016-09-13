using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageRoutingInfoByCustomEntityIdQueryHandler 
        : IAsyncQueryHandler<GetPageRoutingInfoByCustomEntityIdQuery, IEnumerable<PageRoutingInfo>>
        , IPermissionRestrictedQueryHandler<GetPageRoutingInfoByCustomEntityIdQuery, IEnumerable<PageRoutingInfo>>
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetPageRoutingInfoByCustomEntityIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<IEnumerable<PageRoutingInfo>> ExecuteAsync(GetPageRoutingInfoByCustomEntityIdQuery query, IExecutionContext executionContext)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetPageRoutingInfoByCustomEntityIdRangeQuery(query.CustomEntityDefinitionCode, new int[] { query.CustomEntityId }), executionContext);

            if (!result.ContainsKey(query.CustomEntityId)) return Enumerable.Empty<PageRoutingInfo>();

            return result[query.CustomEntityId].OrderBy(e => e.PageRoute.UrlPath.Length);
        }

        private PageRoutingInfo ToRoutingInfo(PageRoute pageRoute, CustomEntityRoute customEntityRoute, ICustomEntityRoutingRule rule = null)
        {
            return new PageRoutingInfo()
            {
                PageRoute = pageRoute,
                CustomEntityRoute = customEntityRoute,
                CustomEntityRouteRule = rule
            };
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutingInfoByCustomEntityIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }

}
