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
    public class GetPageRoutesByCustomEntityDefinitionCodeQueryHandler
        : IAsyncQueryHandler<GetPageRoutesByCustomEntityDefinitionCodeQuery, IEnumerable<PageRoute>>
        , IPermissionRestrictedQueryHandler<GetPageRoutesByCustomEntityDefinitionCodeQuery, IEnumerable<PageRoute>>
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public GetPageRoutesByCustomEntityDefinitionCodeQueryHandler(
            IQueryExecutor queryExecutor,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _queryExecutor = queryExecutor;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        public async Task<IEnumerable<PageRoute>> ExecuteAsync(GetPageRoutesByCustomEntityDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            var allRoutes = await _queryExecutor.GetAllAsync<PageRoute>(executionContext);
            var customEntityRoutes = allRoutes
                .Where(p => p.CustomEntityDefinitionCode == query.CustomEntityDefinitionCode)
                .OrderBy(p => p.Locale != null)
                .ThenBy(p => p.Title)
                .ToList();

            return customEntityRoutes;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutesByCustomEntityDefinitionCodeQuery query)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            yield return new CustomEntityReadPermission(definition);
            yield return new PageReadPermission();
        }

        #endregion
    }

}
