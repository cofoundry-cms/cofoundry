using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using System.Data.Entity;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class GetCustomEntityVersionSummariesByCustomEntityIdQueryHandler 
        : IAsyncQueryHandler<GetCustomEntityVersionSummariesByCustomEntityIdQuery, IEnumerable<CustomEntityVersionSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntityVersionSummariesByCustomEntityIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public async Task<IEnumerable<CustomEntityVersionSummary>> ExecuteAsync(GetCustomEntityVersionSummariesByCustomEntityIdQuery query, IExecutionContext executionContext)
        {
            var definitionCode = await _dbContext
                .CustomEntities
                .AsNoTracking()
                .Where(c => c.CustomEntityId == query.CustomEntityId)
                .Select(c => c.CustomEntityDefinitionCode)
                .FirstOrDefaultAsync();
            if (definitionCode == null) return null;

            await _permissionValidationService.EnforceCustomEntityPermissionAsync<CustomEntityReadPermission>(definitionCode);

            var versions = await Query(query.CustomEntityId).ToListAsync();
            if (versions == null) return null;

            return versions;
        }

        #endregion

        #region helpers

        private IQueryable<CustomEntityVersionSummary> Query(int id)
        {
            return _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .Where(v => v.CustomEntityId == id)
                .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                .ThenByDescending(v => v.CreateDate)
                .ProjectTo<CustomEntityVersionSummary>();
        }

        #endregion
    }
}
