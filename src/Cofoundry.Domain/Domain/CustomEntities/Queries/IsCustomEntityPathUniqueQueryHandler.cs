using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class IsCustomEntityPathUniqueQueryHandler 
        : IQueryHandler<IsCustomEntityPathUniqueQuery, bool>
        , IAsyncQueryHandler<IsCustomEntityPathUniqueQuery, bool>
        , IPermissionRestrictedQueryHandler<IsCustomEntityPathUniqueQuery, bool>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICustomEntityCodeDefinitionRepository _customEntityDefinitionRepository;

        public IsCustomEntityPathUniqueQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ICustomEntityCodeDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }
        
        #endregion

        #region execution

        public async Task<bool> ExecuteAsync(IsCustomEntityPathUniqueQuery query, IExecutionContext executionContext)
        {
            var definition = GetDefinition(query);
            if (!definition.ForceUrlSlugUniqueness) return true;

            var dbQuery = Query(query);
            var exisits = await dbQuery.AnyAsync();

            return !exisits;
        }

        public bool Execute(IsCustomEntityPathUniqueQuery query, IExecutionContext executionContext)
        {
            var definition = GetDefinition(query);
            if (!definition.ForceUrlSlugUniqueness) return true;

            var dbQuery = Query(query);
            var exisits = dbQuery.Any();

            return !exisits;
        }

        #endregion

        #region private helpers

        private IQueryable<CustomEntity> Query(IsCustomEntityPathUniqueQuery query)
        {
            var dbQuery = _dbContext
                .CustomEntities
                .AsNoTracking()
                .Where(d => d.CustomEntityId != query.CustomEntityId
                    && d.CustomEntityDefinitionCode == query.CustomEntityDefinitionCode
                    && d.UrlSlug == query.UrlSlug
                    && d.LocaleId == query.LocaleId
                    );

            return dbQuery;
        }

        private CustomEntityDefinitionSummary GetDefinition(IsCustomEntityPathUniqueQuery query)
        {
            var definition = _queryExecutor.GetById<CustomEntityDefinitionSummary>(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);
            return definition;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(IsCustomEntityPathUniqueQuery query)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            yield return new CustomEntityReadPermission(definition);
        }

        #endregion
    }

}
