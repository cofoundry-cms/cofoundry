using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using AutoMapper;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets CustomEntityRoute data for all custom entities of a 
    /// specific type. These route objects are small and cached which
    /// makes them good for quick lookups.
    /// </summary>
    public class GetCustomEntityRoutesByDefinitionCodeQueryHandler 
        : IQueryHandler<GetCustomEntityRoutesByDefinitionCodeQuery, IEnumerable<CustomEntityRoute>>
        , IAsyncQueryHandler<GetCustomEntityRoutesByDefinitionCodeQuery, IEnumerable<CustomEntityRoute>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly CustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public GetCustomEntityRoutesByDefinitionCodeQueryHandler(
            CofoundryDbContext dbContext,
            ICustomEntityCache customEntityCache,
            CustomEntityDataModelMapper customEntityDataModelMapper,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _customEntityCache = customEntityCache;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
            _customEntityDataModelMapper = customEntityDataModelMapper;
        }

        #endregion

        public async Task<IEnumerable<CustomEntityRoute>> ExecuteAsync(GetCustomEntityRoutesByDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            return await _customEntityCache.GetOrAddAsync(query.CustomEntityDefinitionCode, async () =>
            {
                var dbRoutes = await GetDbQuery(query).ToListAsync();
                return MapRoutes(query, dbRoutes);;
            });
        }

        public IEnumerable<CustomEntityRoute> Execute(GetCustomEntityRoutesByDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            return _customEntityCache.GetOrAdd(query.CustomEntityDefinitionCode, () =>
            {
                var dbRoutes = GetDbQuery(query).ToList();
                return MapRoutes(query, dbRoutes);
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

        private CustomEntityRoute[] MapRoutes(GetCustomEntityRoutesByDefinitionCodeQuery query, List<CustomEntity> dbRoutes)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            var routingDataProperties = definition
                .GetDataModelType()
                .GetProperties()
                .Where(prop => prop.IsDefined(typeof(CustomEntityRouteDataAttribute), false));

            var routes = new List<CustomEntityRoute>(dbRoutes.Count);

            foreach (var dbRoute in dbRoutes)
            {
                var route = Mapper.Map<CustomEntityRoute>(dbRoute);
                var versions = new List<CustomEntityVersionRoute>();
                route.Versions = versions;
                routes.Add(route);

                foreach (var dbVersion in dbRoute.CustomEntityVersions)
                {
                    var version = Mapper.Map<CustomEntityVersionRoute>(dbVersion);
                    versions.Add(version);

                    if (routingDataProperties.Any())
                    {
                        // Parse additional routing data properties
                        var model = _customEntityDataModelMapper.Map(query.CustomEntityDefinitionCode, dbVersion.SerializedData);
                        foreach (var routingDataProperty in routingDataProperties)
                        {
                            version.AdditionalRoutingData.Add(routingDataProperty.Name, Convert.ToString(routingDataProperty.GetValue(model)));
                        }
                    }
                }
            }

            return routes.ToArray();
        }
    }
}
