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
    public class GetCustomEntityDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<CustomEntityDetails>, CustomEntityDetails>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<CustomEntityDetails> ExecuteAsync(GetByIdQuery<CustomEntityDetails> query, IExecutionContext executionContext)
        {
            var customEntityVersion = await Query(query.Id).FirstOrDefaultAsync();
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(customEntityVersion.CustomEntity.CustomEntityDefinitionCode);

            return await Map(query, customEntityVersion);
        }

        #endregion

        #region helpers

        private async Task<CustomEntityDetails> Map(GetByIdQuery<CustomEntityDetails> query, CustomEntityVersion dbVersion)
        {
            if (dbVersion == null) return null;

            var entity = Mapper.Map<CustomEntityDetails>(dbVersion.CustomEntity);
            entity.LatestVersion = Mapper.Map<CustomEntityVersionDetails>(dbVersion);
            entity.HasDraft = entity.LatestVersion.WorkFlowStatus == WorkFlowStatus.Draft;
            entity.IsPublished = entity.LatestVersion.WorkFlowStatus == WorkFlowStatus.Published;

            if (!entity.IsPublished)
            {
                entity.IsPublished = await _dbContext
                    .CustomEntityVersions
                    .AnyAsync(v => v.CustomEntityId == query.Id && v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
            }

            // Custom Mapping
            MapDataModel(query, dbVersion, entity.LatestVersion);
            
            await MapFullPath(dbVersion, entity);

            return entity;
        }

        private async Task MapFullPath(CustomEntityVersion dbVersion, CustomEntityDetails entity)
        {
            var routingsQuery = new GetPageRoutingInfoByCustomEntityIdQuery(dbVersion.CustomEntity.CustomEntityDefinitionCode, dbVersion.CustomEntityId);
            var routings = await _queryExecutor.ExecuteAsync(routingsQuery);
            var detailsRouting = routings.FirstOrDefault(r => r.CustomEntityRouteRule != null);
            if (detailsRouting != null)
            {
                entity.FullPath = detailsRouting.CustomEntityRouteRule.MakeUrl(detailsRouting.PageRoute, detailsRouting.CustomEntityRoute);
            }
        }

        private IQueryable<CustomEntityVersion> Query(int id)
        {
            return _dbContext
                .CustomEntityVersions
                .Include(v => v.CustomEntity)
                .Include(v => v.CustomEntity.Creator)
                .Include(v => v.CustomEntity.Locale)
                .Include(v => v.Creator)
                .AsNoTracking()
                .Where(v => v.CustomEntityId == id && (v.CustomEntity.LocaleId == null || v.CustomEntity.Locale.IsActive))
                .OrderByDescending(g => g.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(g => g.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                .ThenByDescending(g => g.CreateDate);
        }

        private void MapDataModel(GetByIdQuery<CustomEntityDetails> query, CustomEntityVersion dbVersion, CustomEntityVersionDetails version)
        {
            var definition = _queryExecutor.GetById<CustomEntityDefinitionSummary>(dbVersion.CustomEntity.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, dbVersion.CustomEntity.CustomEntityDefinitionCode);

            version.Model = (ICustomEntityVersionDataModel)_dbUnstructuredDataSerializer.Deserialize(dbVersion.SerializedData, definition.DataModelType);
        }

        #endregion
    }
}
