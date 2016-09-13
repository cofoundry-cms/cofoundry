using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRouteByPathQueryHandler 
        : IAsyncQueryHandler<GetCustomEntityRouteByPathQuery, CustomEntityRoute>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public GetCustomEntityRouteByPathQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        public async Task<CustomEntityRoute> ExecuteAsync(GetCustomEntityRouteByPathQuery query, IExecutionContext executionContext)
        {
            var routes = await _queryExecutor.ExecuteAsync(new GetCustomEntityRoutesByDefinitionCodeQuery(query.CustomEntityDefinitionCode));

            var filter = routes.Where(r => (r.Locale == null && !query.LocaleId.HasValue) || (r.Locale != null && r.Locale.LocaleId == query.LocaleId.Value));

            if (query.CustomEntityId.HasValue)
            {
                return filter.SingleOrDefault(r => r.CustomEntityId == query.CustomEntityId.Value);
            }
            else
            {
                return filter.SingleOrDefault(r => r.UrlSlug.Equals(query.UrlSlug, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
