using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using System.Data.Entity;

namespace Cofoundry.Domain
{
    public class GetPageTemplateDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<PageTemplateDetails>, PageTemplateDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<PageTemplateDetails>, PageTemplateDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;

        public GetPageTemplateDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
        }

        #endregion

        public async Task<PageTemplateDetails> ExecuteAsync(GetByIdQuery<PageTemplateDetails> query, IExecutionContext executionContext)
        {
            var template = await _dbContext
                .PageTemplates
                .AsNoTracking()
                .Where(l => l.PageTemplateId == query.Id)
                .ProjectTo<PageTemplateDetails>()
                .SingleOrDefaultAsync();

            if (template == null) return null;

            // Re-map to code defined version
            if (template.CustomEntityDefinition != null)
            {
                template.CustomEntityDefinition = _queryExecutor.GetById<CustomEntityDefinitionMicroSummary>(template.CustomEntityDefinition.CustomEntityDefinitionCode);
            }

            var sectionQuery = new GetPageTemplateSectionsByPageTemplateIdQuery() { PageTemplateId = query.Id };
            template.Sections = (await _queryExecutor.ExecuteAsync(sectionQuery)).ToArray();

            return template;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<PageTemplateDetails> query)
        {
            yield return new PageTemplateReadPermission();
        }

        #endregion
    }
}
