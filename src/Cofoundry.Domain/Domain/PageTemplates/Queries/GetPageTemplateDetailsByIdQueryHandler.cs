using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.QueryModels;

namespace Cofoundry.Domain
{
    public class GetPageTemplateDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<PageTemplateDetails>, PageTemplateDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<PageTemplateDetails>, PageTemplateDetails>
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

        public async Task<PageTemplateDetails> ExecuteAsync(GetByIdQuery<PageTemplateDetails> query, IExecutionContext executionContext)
        {
            var queryModel = new PageTemplateDetailsQueryModel();

            queryModel.PageTemplate = await _dbContext
                .PageTemplates
                .AsNoTracking()
                .Include(t => t.PageTemplateRegions)
                .Where(l => l.PageTemplateId == query.Id)
                .SingleOrDefaultAsync();

            if (queryModel.PageTemplate == null) return null;

            queryModel.CustomEntityDefinition = await _queryExecutor.GetByIdAsync<CustomEntityDefinitionMicroSummary>(queryModel.PageTemplate.CustomEntityDefinitionCode);
            queryModel.NumPages = await _dbContext
                .PageVersions
                .AsNoTracking()
                .Where(v => v.PageTemplateId == query.Id && !v.Page.IsDeleted && !v.IsDeleted)
                .GroupBy(v => v.PageId)
                .CountAsync();

            var template = _pageTemplateDetailsMapper.Map(queryModel);

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
