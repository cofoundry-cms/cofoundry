using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using System.Data.Entity;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRenderSummaryByDefinitionCodeQueryHandler 
        : IQueryHandler<GetCustomEntityRenderSummaryByDefinitionCodeQuery, IEnumerable<CustomEntityRenderSummary>>
        , IAsyncQueryHandler<GetCustomEntityRenderSummaryByDefinitionCodeQuery, IEnumerable<CustomEntityRenderSummary>>
        , IPermissionRestrictedQueryHandler<GetCustomEntityRenderSummaryByDefinitionCodeQuery, IEnumerable<CustomEntityRenderSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly CustomEntityDataModelMapper _customEntityDataModelMapper;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICustomEntityCodeDefinitionRepository _customEntityDefinitionRepository;

        public GetCustomEntityRenderSummaryByDefinitionCodeQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            CustomEntityDataModelMapper customEntityDataModelMapper,
            ICustomEntityCodeDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _customEntityDataModelMapper = customEntityDataModelMapper;
            _queryExecutor = queryExecutor;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<IEnumerable<CustomEntityRenderSummary>> ExecuteAsync(GetCustomEntityRenderSummaryByDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            var dbResults = await Query(query).ToListAsync();
            var results = Map(dbResults).ToList();

            return results;
        }

        public IEnumerable<CustomEntityRenderSummary> Execute(GetCustomEntityRenderSummaryByDefinitionCodeQuery query, IExecutionContext executionContext)
        {
            var dbResults = Query(query).ToList();
            var results = Map(dbResults).ToList();

            return results;
        }

        #endregion

        #region private helpers

        private IQueryable<CustomEntityVersion> Query(GetCustomEntityRenderSummaryByDefinitionCodeQuery query)
        {
            var dbQuery = _dbContext
                .CustomEntityVersions
                .AsNoTracking()
                .FilterByCustomEntityDefinitionCode(query.CustomEntityDefinitionCode)
                .FilterByWorkFlowStatusQuery(query.WorkFlowStatus)
                .Include(e => e.CustomEntity)
                .Include(e => e.CustomEntity.Locale);

            return dbQuery;
        }

        private IEnumerable<CustomEntityRenderSummary> Map(List<CustomEntityVersion> dbResults)
        {
            foreach (var dbResult in dbResults)
            {
                var entity = Mapper.Map<CustomEntityRenderSummary>(dbResult);
                entity.Model = _customEntityDataModelMapper.Map(dbResult.CustomEntity.CustomEntityDefinitionCode, dbResult.SerializedData);
                yield return entity;
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetCustomEntityRenderSummaryByDefinitionCodeQuery query)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            yield return new CustomEntityReadPermission(definition);
        }

        #endregion
    }
}
