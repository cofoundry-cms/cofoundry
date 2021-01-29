using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class GetAllPageTemplateMicroSummariesQueryHandler 
        : IQueryHandler<GetAllPageTemplateMicroSummariesQuery, ICollection<PageTemplateMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetAllPageTemplateMicroSummariesQuery, ICollection<PageTemplateMicroSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageTemplateMicroSummaryMapper _pageTemplateMapper;

        public GetAllPageTemplateMicroSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IPageTemplateMicroSummaryMapper pageTemplateMapper
            )
        {
            _dbContext = dbContext;
            _pageTemplateMapper = pageTemplateMapper;
        }

        #endregion

        #region execution
        
        public async Task<ICollection<PageTemplateMicroSummary>> ExecuteAsync(GetAllPageTemplateMicroSummariesQuery query, IExecutionContext executionContext)
        {
            var dbResults = await Query().ToListAsync();
            var results = dbResults
                .Select(_pageTemplateMapper.Map)
                .ToList();

            return results;
        }

        private IQueryable<PageTemplate> Query()
        {
            return _dbContext
                .PageTemplates
                .AsNoTracking()
                .FilterActive()
                .OrderBy(l => l.FileName);
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllPageTemplateMicroSummariesQuery query)
        {
            yield return new PageTemplateReadPermission();
        }

        #endregion
    }
}
