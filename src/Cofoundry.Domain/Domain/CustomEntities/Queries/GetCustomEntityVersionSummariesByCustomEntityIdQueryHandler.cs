using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Gets a set of custom entity versions for a specific 
    /// custom entity, ordered by create date, and returns
    /// them as a paged collection of CustomEntityVersionSummary
    /// projections.
    /// </summary>
    public class GetCustomEntityVersionSummariesByCustomEntityIdQueryHandler 
        : IQueryHandler<GetCustomEntityVersionSummariesByCustomEntityIdQuery, PagedQueryResult<CustomEntityVersionSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ICustomEntityVersionSummaryMapper _customEntityVersionSummaryMapper;

        public GetCustomEntityVersionSummariesByCustomEntityIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPermissionValidationService permissionValidationService,
            ICustomEntityVersionSummaryMapper customEntityVersionSummaryMapper
            )
        {
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
            _customEntityVersionSummaryMapper = customEntityVersionSummaryMapper;
        }

        #endregion

        public async Task<PagedQueryResult<CustomEntityVersionSummary>> ExecuteAsync(GetCustomEntityVersionSummariesByCustomEntityIdQuery query, IExecutionContext executionContext)
        {
            var definitionCode = await _dbContext
                .CustomEntities
                .AsNoTracking()
                .Where(c => c.CustomEntityId == query.CustomEntityId)
                .Select(c => c.CustomEntityDefinitionCode)
                .FirstOrDefaultAsync();

            if (definitionCode == null) return null;

            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(definitionCode, executionContext.UserContext);

            var dbVersions = await Query(query.CustomEntityId).ToPagedResultAsync(query);
            var versions = _customEntityVersionSummaryMapper.MapVersions(query.CustomEntityId, dbVersions);
            
            return versions;
        }

        private IQueryable<CustomEntityVersion> Query(int id)
        {
            return _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .Include(e => e.Creator)
                .FilterActive()
                .FilterByCustomEntityId(id)
                .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(v => v.CreateDate);
        }
    }
}
