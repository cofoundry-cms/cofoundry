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

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Gets CustomEntityRoute data for all custom entities of a 
    /// specific type. These route objects are small and cached which
    /// makes them good for quick lookups.
    /// </summary>
    public class GetCustomEntityRoutesByDefinitionCodeQueryHandler
        : IQueryHandler<GetCustomEntityRoutesByDefinitionCodeQuery, ICollection<CustomEntityRoute>>
        , IIgnorePermissionCheckHandler
    {
        private static readonly MethodInfo _mapAdditionalRouteDataAsyncMethod = typeof(GetCustomEntityRoutesByDefinitionCodeQueryHandler).GetMethod(nameof(MapAdditionalRouteDataAsync), BindingFlags.NonPublic | BindingFlags.Instance);

        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly ICustomEntityCache _customEntityCache;
        private readonly ICustomEntityRouteMapper _customEntityRouteMapper;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly ICustomEntityRouteDataBuilderFactory _customEntityRouteDataBuilderFactory;

        public GetCustomEntityRoutesByDefinitionCodeQueryHandler(
            CofoundryDbContext dbContext,
            ICustomEntityCache customEntityCache,
            ICustomEntityRouteMapper customEntityRouteMapper,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository,
            IQueryExecutor queryExecutor,
            ICustomEntityDataModelMapper customEntityDataModelMapper,
            ICustomEntityRouteDataBuilderFactory customEntityRouteDataBuilderFactory
            )
        {
            _dbContext = dbContext;
            _customEntityCache = customEntityCache;
            _customEntityRouteMapper = customEntityRouteMapper;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
            _queryExecutor = queryExecutor;
            _customEntityDataModelMapper = customEntityDataModelMapper;
            _customEntityRouteDataBuilderFactory = customEntityRouteDataBuilderFactory;
        }

        #endregion

        public async Task<ICollection<CustomEntityRoute>> ExecuteAsync(GetCustomEntityRoutesByDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            return await _customEntityCache.GetOrAddAsync(query.CustomEntityDefinitionCode, async () =>
            {
                var dbRoutes = await _dbContext
                    .CustomEntities
                    .Include(c => c.CustomEntityVersions)
                    .Include(c => c.Locale)
                    .AsNoTracking()
                    .Where(e => e.CustomEntityDefinitionCode == query.CustomEntityDefinitionCode && (e.LocaleId == null || e.Locale.IsActive))
                    .ToListAsync();

                var allLocales = await _queryExecutor.ExecuteAsync(new GetAllActiveLocalesQuery(), executionContext);
                var localesLookup = allLocales.ToDictionary(l => l.LocaleId);

                return await MapRoutesAsync(query, dbRoutes, localesLookup); ;
            });
        }

        private async Task<ICollection<CustomEntityRoute>> MapRoutesAsync(
            GetCustomEntityRoutesByDefinitionCodeQuery query,
            List<CustomEntity> dbEntities,
            Dictionary<int, ActiveLocale> allLocales
            )
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            var routes = dbEntities
                .Select(r => MapRoute(r, allLocales))
                .ToList();

            // Map additional parameters

            await (Task)_mapAdditionalRouteDataAsyncMethod
                .MakeGenericMethod(definition.GetType(), definition.GetDataModelType())
                .Invoke(this, new object[] { definition, routes, dbEntities });

            return routes;
        }

        private async Task MapAdditionalRouteDataAsync<TCustomEntityDefinition, TDataModel>(
            TCustomEntityDefinition customEntityDefiniton,
            List<CustomEntityRoute> routes,
            List<CustomEntity> dbEntities
            )
            where TCustomEntityDefinition : ICustomEntityDefinition<TDataModel>
            where TDataModel : ICustomEntityDataModel
        {
            var routeDataBuilders = _customEntityRouteDataBuilderFactory.Create<TCustomEntityDefinition, TDataModel>();

            var routingDataProperties = customEntityDefiniton
                .GetDataModelType()
                .GetTypeInfo()
                .GetProperties()
                .Where(prop => prop.IsDefined(typeof(CustomEntityRouteDataAttribute), false));

            if (!routeDataBuilders.Any() && !routingDataProperties.Any()) return;

            var dbVersionIndex = dbEntities
                .SelectMany(e => e.CustomEntityVersions)
                .ToDictionary(r => r.CustomEntityVersionId);

            var allBuilderParams = new List<CustomEntityRouteDataBuilderParameter<TDataModel>>();

            foreach (var route in routes)
                foreach (var versionRoute in route.Versions)
                {
                    var dbCustomEntityVersion = dbVersionIndex.GetOrDefault(versionRoute.VersionId);

                    if (dbCustomEntityVersion == null)
                    {
                        throw new Exception($"Custom entity {customEntityDefiniton.CustomEntityDefinitionCode}:{route.CustomEntityId} should be in collection, but could not be found");
                    }

                    var dataModel = _customEntityDataModelMapper.Map(customEntityDefiniton.CustomEntityDefinitionCode, dbCustomEntityVersion.SerializedData);

                    if (dataModel == null)
                    {
                        throw new Exception($"Data model should not be null.");
                    }

                    if (!(dataModel is TDataModel))
                    {
                        throw new Exception($"Data model is not of the expected type. Expected {typeof(TDataModel).FullName}, got {dataModel.GetType().FullName}");
                    }

                    var builderParam = new CustomEntityRouteDataBuilderParameter<TDataModel>(
                        route,
                        versionRoute,
                        (TDataModel)dataModel
                    );

                    allBuilderParams.Add(builderParam);

                    // Bind routing data properties 
                    foreach (var routingDataProperty in routingDataProperties)
                    {
                        var value = Convert.ToString(routingDataProperty.GetValue(dataModel));
                        builderParam.AdditionalRoutingData.Add(routingDataProperty.Name, value);
                    }
                }

            // Run injected route builders
            foreach (var routeDataBuilder in routeDataBuilders)
            {
                await routeDataBuilder.BuildAsync(allBuilderParams);
            }
        }

        public CustomEntityRoute MapRoute(
            CustomEntity dbCustomEntity,
            Dictionary<int, ActiveLocale> allLocales
            )
        {
            ActiveLocale locale = null;

            if (dbCustomEntity.LocaleId.HasValue)
            {
                locale = allLocales.GetOrDefault(dbCustomEntity.LocaleId.Value);
            }

            return _customEntityRouteMapper.Map(dbCustomEntity, locale);
        }
    }
}
