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
        private readonly ICustomEntityRenderSummaryMapper _customEntityRenderSummaryMapper;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityRenderSummaryByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor, 
            ICustomEntityRenderSummaryMapper customEntityRenderSummaryMapper,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _customEntityRenderSummaryMapper = customEntityRenderSummaryMapper;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<CustomEntityRenderSummary> ExecuteAsync(GetCustomEntityRenderSummaryByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await Query(query).SingleOrDefaultAsync();
            if (dbResult == null) return null;
            EnforcePermission(dbResult);
            var result = await _customEntityRenderSummaryMapper.MapSummaryAsync(dbResult, executionContext);

            return result;
        }

        public CustomEntityRenderSummary Execute(GetCustomEntityRenderSummaryByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = Query(query).SingleOrDefault();
            if (dbResult == null) return null;
            EnforcePermission(dbResult);
            var result = _customEntityRenderSummaryMapper.MapSummary(dbResult, executionContext);

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
                .Include(e => e.CustomEntity);

            return dbQuery;
        }

        private void EnforcePermission(CustomEntityVersion dbResult)
        {
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(dbResult.CustomEntity.CustomEntityDefinitionCode);
        }

        #endregion
    }
}
