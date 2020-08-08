using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Determines if the UrlSlug property for a custom entity is invalid because it
    /// already exists. If the custom entity defition has ForceUrlSlugUniqueness 
    /// set to true then duplicates are not permitted, otherwise true will always
    /// be returned.
    /// </summary>
    public class IsCustomEntityUrlSlugUniqueQueryHandler
        : IQueryHandler<IsCustomEntityUrlSlugUniqueQuery, bool>
        , IPermissionRestrictedQueryHandler<IsCustomEntityUrlSlugUniqueQuery, bool>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public IsCustomEntityUrlSlugUniqueQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }
        
        #endregion

        public async Task<bool> ExecuteAsync(IsCustomEntityUrlSlugUniqueQuery query, IExecutionContext executionContext)
        {
            var definition = await GetDefinitionAsync(query, executionContext);
            if (!definition.ForceUrlSlugUniqueness) return true;

            var dbQuery = Query(query);
            var exisits = await dbQuery.AnyAsync();

            return !exisits;
        }

        private IQueryable<CustomEntity> Query(IsCustomEntityUrlSlugUniqueQuery query)
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

        private async Task<CustomEntityDefinitionSummary> GetDefinitionAsync(
            IsCustomEntityUrlSlugUniqueQuery query,
            IExecutionContext executionContext
            )
        {
            var definitionQuery = new GetCustomEntityDefinitionSummaryByCodeQuery(query.CustomEntityDefinitionCode);
            var definition = await _queryExecutor.ExecuteAsync(definitionQuery, executionContext);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            return definition;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(IsCustomEntityUrlSlugUniqueQuery query)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, query.CustomEntityDefinitionCode);

            yield return new CustomEntityReadPermission(definition);
        }

        #endregion
    }

}
