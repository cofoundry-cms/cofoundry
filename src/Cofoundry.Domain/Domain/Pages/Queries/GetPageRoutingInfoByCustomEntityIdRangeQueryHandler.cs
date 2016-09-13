using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetPageRoutingInfoByCustomEntityIdRangeQueryHandler 
        : IAsyncQueryHandler<GetPageRoutingInfoByCustomEntityIdRangeQuery, IDictionary<int, IEnumerable<PageRoutingInfo>>>
        , IPermissionRestrictedQueryHandler<GetPageRoutingInfoByCustomEntityIdRangeQuery, IDictionary<int, IEnumerable<PageRoutingInfo>>>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;

        public GetPageRoutingInfoByCustomEntityIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
        }


        public async Task<IDictionary<int, IEnumerable<PageRoutingInfo>>> ExecuteAsync(GetPageRoutingInfoByCustomEntityIdRangeQuery query, IExecutionContext executionContext)
        {
            var idSets = await _dbContext
                .Pages
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .Join(_dbContext.CustomEntities, p => new { p.CustomEntityDefinitionCode, p.LocaleId }, e => new { e.CustomEntityDefinitionCode, e.LocaleId }, (p, e) => new
                {
                    CustomEntity = e,
                    Page = p
                })
                .Where(r => query.Ids.Contains(r.CustomEntity.CustomEntityId) && r.CustomEntity.CustomEntityDefinitionCode == query.CustomEntityDefinitionCode)
                .Select(r => new
                {
                    r.Page.PageId,
                    r.CustomEntity.CustomEntityId
                })
                .ToListAsync();

            var pageRoutes = await _queryExecutor.GetByIdRangeAsync<PageRoute>(idSets.Select(p => p.PageId), executionContext);
            var customEntityRoutes = await _queryExecutor.ExecuteAsync(new GetCustomEntityRoutesByDefinitionCodeQuery(query.CustomEntityDefinitionCode), executionContext);

            var allRules = _queryExecutor.GetAll<ICustomEntityRoutingRule>(executionContext);

            var result = new List<PageRoutingInfo>(idSets.Count);

            foreach (var idSet in idSets)
            {
                var routingInfo = new PageRoutingInfo();

                routingInfo.PageRoute = pageRoutes.GetOrDefault(idSet.PageId);
                EntityNotFoundException.ThrowIfNull(routingInfo.PageRoute, idSet.PageId);

                routingInfo.CustomEntityRoute = customEntityRoutes.SingleOrDefault(e => e.CustomEntityId == idSet.CustomEntityId);
                EntityNotFoundException.ThrowIfNull(routingInfo.CustomEntityRoute, idSet.PageId);

                routingInfo.CustomEntityRouteRule = allRules.FirstOrDefault(r => r.RouteFormat == routingInfo.PageRoute.UrlPath);

                result.Add(routingInfo);
            }

            return result
                .GroupBy(r => r.CustomEntityRoute.CustomEntityId)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
        }
        
        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutingInfoByCustomEntityIdRangeQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }

}
