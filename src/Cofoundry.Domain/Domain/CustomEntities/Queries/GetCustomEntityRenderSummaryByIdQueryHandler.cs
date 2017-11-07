using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRenderSummaryByIdQueryHandler 
        : IAsyncQueryHandler<GetCustomEntityRenderSummaryByIdQuery, CustomEntityRenderSummary>
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
            var dbResult = await QueryAsync(query, executionContext);
            if (dbResult == null) return null;

            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityReadPermission>(dbResult.CustomEntity.CustomEntityDefinitionCode);

            var result = await _customEntityRenderSummaryMapper.MapAsync(dbResult, executionContext);

            return result;
        }

        private async Task<CustomEntityVersion> QueryAsync(GetCustomEntityRenderSummaryByIdQuery query, IExecutionContext executionContext)
        {
            CustomEntityVersion result;

            if (query.PublishStatus == PublishStatusQuery.SpecificVersion)
            {
                if (!query.CustomEntityVersionId.HasValue)
                {
                    throw new Exception("A CustomEntityVersionId must be included in the query to use PublishStatusQuery.SpecificVersion");
                }

                result = await _dbContext
                    .CustomEntityVersions
                    .AsNoTracking()
                    .Include(e => e.CustomEntity)
                    .FilterByActive()
                    .FilterByCustomEntityId(query.CustomEntityId)
                    .FilterByCustomEntityVersionId(query.CustomEntityVersionId.Value)
                    .SingleOrDefaultAsync();
            }
            else
            {
                var dbResult = await _dbContext
                    .CustomEntityPublishStatusQueries
                    .AsNoTracking()
                    .Include(e => e.CustomEntityVersion)
                    .ThenInclude(e => e.CustomEntity)
                    .FilterByActive()
                    .FilterByCustomEntityId(query.CustomEntityId)
                    .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
                    .SingleOrDefaultAsync();

                result = dbResult?.CustomEntityVersion;
            }

            return result;
        }

        #endregion
    }
}
