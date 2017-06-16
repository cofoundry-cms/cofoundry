using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class GetCustomEntitySummaryByIdRangeQueryHandler 
        : IQueryHandler<GetByIdRangeQuery<CustomEntitySummary>, IDictionary<int, CustomEntitySummary>>
        , IAsyncQueryHandler<GetByIdRangeQuery<CustomEntitySummary>, IDictionary<int, CustomEntitySummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly CustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetCustomEntitySummaryByIdRangeQueryHandler(
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

        public IDictionary<int, CustomEntitySummary> Execute(GetByIdRangeQuery<CustomEntitySummary> query, IExecutionContext executionContext)
        {
            var dbResults = Query(query.Ids).ToList();
            var results = Map(dbResults, query.Ids, executionContext);

            return results;
        }

        public async Task<IDictionary<int, CustomEntitySummary>> ExecuteAsync(GetByIdRangeQuery<CustomEntitySummary> query, IExecutionContext executionContext)
        {
            var dbResults = await Query(query.Ids).ToListAsync();
            var results = Map(dbResults, query.Ids, executionContext);

            return results;
        }

        #endregion

        #region private helpers

        private IQueryable<CustomEntitySummaryQueryModel> Query(int[] ids)
        {
            var dbQuery = _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .Include(e => e.CustomEntity)
                .ThenInclude(e => e.Locale)
                .Where(v => ids.Contains(v.CustomEntityId))
                .Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft || v.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                .GroupBy(e => e.CustomEntityId, (key, g) => g.OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft).FirstOrDefault())
                .ProjectTo<CustomEntitySummaryQueryModel>();

            return dbQuery;
        }

        private Dictionary<int, CustomEntitySummary> Map(List<CustomEntitySummaryQueryModel> dbResults, int[] ids, IExecutionContext executionContext)
        {
            var results = new Dictionary<int, CustomEntitySummary>(dbResults.Count);

            var definitionCodes = dbResults.Select(r => r.CustomEntityDefinitionCode);
            _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(definitionCodes, executionContext.UserContext);

            foreach (var dbResult in dbResults)
            {
                var entity = Mapper.Map<CustomEntitySummary>(dbResult);
                entity.Model = _customEntityDataModelMapper.Map(dbResult.CustomEntityDefinitionCode, dbResult.SerializedData);
                results.Add(entity.CustomEntityId, entity);
            }

            return results;
        }

        #endregion
    }
}
