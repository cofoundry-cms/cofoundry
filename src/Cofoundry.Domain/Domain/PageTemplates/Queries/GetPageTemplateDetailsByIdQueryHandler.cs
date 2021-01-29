using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.QueryModels;

namespace Cofoundry.Domain.Internal
{
    public class GetPageTemplateDetailsByIdQueryHandler 
        : IQueryHandler<GetPageTemplateDetailsByIdQuery, PageTemplateDetails>
        , IPermissionRestrictedQueryHandler<GetPageTemplateDetailsByIdQuery, PageTemplateDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageTemplateDetailsMapper _pageTemplateDetailsMapper;

        public GetPageTemplateDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageTemplateDetailsMapper pageTemplateDetailsMapper
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _pageTemplateDetailsMapper = pageTemplateDetailsMapper;
        }

        #endregion

        public async Task<PageTemplateDetails> ExecuteAsync(GetPageTemplateDetailsByIdQuery query, IExecutionContext executionContext)
        {
            var queryModel = new PageTemplateDetailsQueryModel();

            queryModel.PageTemplate = await _dbContext
                .PageTemplates
                .AsNoTracking()
                .Include(t => t.PageTemplateRegions)
                .Where(l => l.PageTemplateId == query.PageTemplateId)
                .SingleOrDefaultAsync();

            if (queryModel.PageTemplate == null) return null;

            if (!string.IsNullOrEmpty(queryModel.PageTemplate.CustomEntityDefinitionCode))
            {
                var definitionQuery = new GetCustomEntityDefinitionMicroSummaryByCodeQuery(queryModel.PageTemplate.CustomEntityDefinitionCode);
                queryModel.CustomEntityDefinition = await _queryExecutor.ExecuteAsync(definitionQuery, executionContext);
            }

            queryModel.NumPages = await _dbContext
                .PageVersions
                .AsNoTracking()
                .Where(v => v.PageTemplateId == query.PageTemplateId)
                .Select(v => v.Page)
                .Distinct()
                .CountAsync();

            var template = _pageTemplateDetailsMapper.Map(queryModel);

            return template;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageTemplateDetailsByIdQuery query)
        {
            yield return new PageTemplateReadPermission();
        }

        #endregion
    }
}
