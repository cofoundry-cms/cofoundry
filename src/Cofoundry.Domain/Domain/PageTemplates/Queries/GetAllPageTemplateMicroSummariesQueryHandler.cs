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
using System.Diagnostics;

namespace Cofoundry.Domain
{
    public class GetAllPageTemplateMicroSummariesQueryHandler 
        : IAsyncQueryHandler<GetAllQuery<PageTemplateMicroSummary>, IEnumerable<PageTemplateMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<PageTemplateMicroSummary>, IEnumerable<PageTemplateMicroSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageTemplateMapper _pageTemplateMapper;

        public GetAllPageTemplateMicroSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IPageTemplateMapper pageTemplateMapper
            )
        {
            _dbContext = dbContext;
            _pageTemplateMapper = pageTemplateMapper;
        }

        #endregion

        #region execution
        
        public async Task<IEnumerable<PageTemplateMicroSummary>> ExecuteAsync(GetAllQuery<PageTemplateMicroSummary> query, IExecutionContext executionContext)
        {
            var dbResults = await Query().ToListAsync();
            var results = Map(dbResults).ToList();

            return results;
        }

        private IQueryable<PageTemplate> Query()
        {
            return _dbContext
                .PageTemplates
                .AsNoTracking()
                .OrderBy(l => l.FileName);
        }

        private IEnumerable<PageTemplateMicroSummary> Map(List<PageTemplate> pageTemplates)
        {
            foreach (var pageTemplate in pageTemplates)
            {
                yield return _pageTemplateMapper.MapMicroSummary(pageTemplate);
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllQuery<PageTemplateMicroSummary> query)
        {
            yield return new PageTemplateReadPermission();
        }

        #endregion
    }
}
