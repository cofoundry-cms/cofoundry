using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRenderSummaryByIdQueryHandler 
        : IQueryHandler<GetCustomEntityRenderSummaryByIdQuery, CustomEntityRenderSummary>
        , IAsyncQueryHandler<GetCustomEntityRenderSummaryByIdQuery, CustomEntityRenderSummary>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly CustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityRenderSummaryByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            CustomEntityDataModelMapper customEntityDataModelMapper,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _customEntityDataModelMapper = customEntityDataModelMapper;
            _queryExecutor = queryExecutor;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<CustomEntityRenderSummary> ExecuteAsync(GetCustomEntityRenderSummaryByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await Query(query).SingleOrDefaultAsync();
            var result = Map(dbResult);

            return result;
        }

        public CustomEntityRenderSummary Execute(GetCustomEntityRenderSummaryByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = Query(query).SingleOrDefault();
            var result = Map(dbResult);

            return result;
        }

        #endregion

        #region private helpers

        private IQueryable<CustomEntityVersion> Query(GetCustomEntityRenderSummaryByIdQuery query)
        {
            var dbQuery = _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .FilterByCustomEntityId(query.CustomEntityId)
                .FilterByWorkFlowStatusQuery(query.WorkFlowStatus)
                .Include(e => e.CustomEntity)
                .Include(e => e.CustomEntity.Locale);
            return dbQuery;
        }

        private CustomEntityRenderSummary Map(CustomEntityVersion dbResult)
        {
            if (dbResult == null) return null;

            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(dbResult.CustomEntity.CustomEntityDefinitionCode);

            var entity = Mapper.Map<CustomEntityRenderSummary>(dbResult);
            entity.Model = _customEntityDataModelMapper.Map(dbResult.CustomEntity.CustomEntityDefinitionCode, dbResult.SerializedData);

            return entity;
        }

        #endregion
    }
}
