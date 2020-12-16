using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Gets a custom entity by it's database id, returning a 
    /// general-purpose CustomEntityRenderSummary projection which
    /// includes version specific data and a deserialized data model. 
    /// The result is  version-sensitive and defaults to returning published 
    /// versions only, but this behavior can be controlled by the 
    /// publishStatus query property.
    /// </summary>
    public class GetCustomEntityRenderSummaryByIdQueryHandler 
        : IQueryHandler<GetCustomEntityRenderSummaryByIdQuery, CustomEntityRenderSummary>
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

        public async Task<CustomEntityRenderSummary> ExecuteAsync(GetCustomEntityRenderSummaryByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await QueryAsync(query, executionContext);
            if (dbResult == null) return null;

            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(dbResult.CustomEntity.CustomEntityDefinitionCode, executionContext.UserContext);

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
                    .FilterActive()
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
                    .FilterActive()
                    .FilterByCustomEntityId(query.CustomEntityId)
                    .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
                    .SingleOrDefaultAsync();

                result = dbResult?.CustomEntityVersion;
            }

            return result;
        }
    }
}
