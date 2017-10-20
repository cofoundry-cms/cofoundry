using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;
using System.Reflection;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets CustomEntityRoute data for all custom entities of a 
    /// specific type. These route objects are small and cached which
    /// makes them good for quick lookups.
    /// </summary>
    public class GetCustomEntityRoutesByDefinitionCodeQueryHandler 
        : IAsyncQueryHandler<GetCustomEntityRoutesByDefinitionCodeQuery, IEnumerable<CustomEntityRoute>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly ICustomEntityRouteMapper _customEntityRouteMapper;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
        private readonly IQueryExecutor _queryExecutor;

        public GetCustomEntityRoutesByDefinitionCodeQueryHandler(
            CofoundryDbContext dbContext,
            ICustomEntityCache customEntityCache,
            ICustomEntityRouteMapper customEntityRouteMapper,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository,
            IQueryExecutor queryExecutor
            )
        {
            _dbContext = dbContext;
            _customEntityCache = customEntityCache;
            _customEntityRouteMapper = customEntityRouteMapper;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
            _queryExecutor = queryExecutor;
        }

        #endregion

        public async Task<IEnumerable<CustomEntityRoute>> ExecuteAsync(GetCustomEntityRoutesByDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            return await _customEntityCache.GetOrAddAsync(query.CustomEntityDefinitionCode, async () =>
            {
                var dbRoutes = await GetDbQuery(query).ToListAsync();
                var allLocales = (await _queryExecutor.GetAllAsync<ActiveLocale>())
                    .ToDictionary(l => l.LocaleId);

                return MapRoutes(query, dbRoutes, allLocales);;
            });
        }

        private IQueryable<CustomEntity> GetDbQuery(GetCustomEntityRoutesByDefinitionCodeQuery query)
        {
            return _dbContext
                    .CustomEntities
                    .Include(c => c.CustomEntityVersions)
                    .Include(c => c.Locale)
                    .AsNoTracking()
                    .Where(e => e.CustomEntityDefinitionCode == query.CustomEntityDefinitionCode && (e.LocaleId == null || e.Locale.IsActive));
        }

        private CustomEntityRoute[] MapRoutes(
            GetCustomEntityRoutesByDefinitionCodeQuery query, 
            List<CustomEntity> dbRoutes,
            Dictionary<int, ActiveLocale> allLocales
            )
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            var routingDataProperties = definition
                .GetDataModelType()
                .GetTypeInfo()
                .GetProperties()
                .Where(prop => prop.IsDefined(typeof(CustomEntityRouteDataAttribute), false));
            
            var routes = dbRoutes
                .Select(r => MapRoute(r, routingDataProperties, allLocales))
                .ToArray();

            return routes;
        }

        public CustomEntityRoute MapRoute(
            CustomEntity dbCustomEntity,
            IEnumerable<PropertyInfo> routingDataProperties,
            Dictionary<int, ActiveLocale> allLocales
            )
        {
            ActiveLocale locale = null;

            if (dbCustomEntity.LocaleId.HasValue)
            {
                locale = allLocales.GetOrDefault(dbCustomEntity.LocaleId.Value);
            }

            return _customEntityRouteMapper.Map(dbCustomEntity, locale, routingDataProperties);
        }
    }
}
