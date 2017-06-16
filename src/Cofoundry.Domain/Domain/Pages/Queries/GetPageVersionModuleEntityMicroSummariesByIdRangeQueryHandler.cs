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
    public class GetPageVersionModuleEntityMicroSummariesByIdRangeQueryHandler 
        : IQueryHandler<GetPageVersionModuleEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IAsyncQueryHandler<GetPageVersionModuleEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetPageVersionModuleEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityDefinitionRepository _entityDefinitionRepository;

        public GetPageVersionModuleEntityMicroSummariesByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IEntityDefinitionRepository entityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _entityDefinitionRepository = entityDefinitionRepository;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetPageVersionModuleEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = await Query(query).ToDictionaryAsync(e => e.ChildEntityId, e => (RootEntityMicroSummary)e);

            return results;
        }

        public IDictionary<int, RootEntityMicroSummary> Execute(GetPageVersionModuleEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
        {
            var results = Query(query).ToDictionary(e => e.ChildEntityId, e => (RootEntityMicroSummary)e);

            return results;
        }

        #endregion

        #region private helpers

        private IQueryable<ChildEntityMicroSummary> Query(GetPageVersionModuleEntityMicroSummariesByIdRangeQuery query)
        {
            var definition = _entityDefinitionRepository.GetByCode(PageEntityDefinition.DefinitionCode);

            var dbQuery = _dbContext
                .PageVersionModules
                .AsNoTracking()
                .Where(m => query.PageVersionModuleIds.Contains(m.PageVersionModuleId))
                .Select(m => new ChildEntityMicroSummary()
                {
                    ChildEntityId = m.PageVersionModuleId,
                    RootEntityId = m.PageVersion.PageId,
                    RootEntityTitle = m.PageVersion.Title,
                    EntityDefinitionCode = definition.EntityDefinitionCode,
                    EntityDefinitionName = definition.Name,
                    IsPreviousVersion = m.PageVersion.WorkFlowStatusId != (int)WorkFlowStatus.Published || m.PageVersion.WorkFlowStatusId != (int)WorkFlowStatus.Draft
                });

            return dbQuery;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageVersionModuleEntityMicroSummariesByIdRangeQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
