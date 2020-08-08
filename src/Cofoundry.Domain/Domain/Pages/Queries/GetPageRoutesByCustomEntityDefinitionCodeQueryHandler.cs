using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public class GetPageRoutesByCustomEntityDefinitionCodeQueryHandler
        : IQueryHandler<GetPageRoutesByCustomEntityDefinitionCodeQuery, ICollection<PageRoute>>
        , IPermissionRestrictedQueryHandler<GetPageRoutesByCustomEntityDefinitionCodeQuery, ICollection<PageRoute>>
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

        public async Task<ICollection<PageRoute>> ExecuteAsync(GetPageRoutesByCustomEntityDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            var allPageRoutes = await _queryExecutor.ExecuteAsync(new GetAllPageRoutesQuery(), executionContext);
            var customEntityRoutes = allPageRoutes
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
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            yield return new CustomEntityReadPermission(definition);
            yield return new PageReadPermission();
        }

        #endregion
    }

}
