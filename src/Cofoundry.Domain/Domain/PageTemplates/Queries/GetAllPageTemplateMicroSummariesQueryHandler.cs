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
using System.Diagnostics;

namespace Cofoundry.Domain
{
    public class GetAllPageTemplateMicroSummariesQueryHandler 
        : IQueryHandler<GetAllQuery<PageTemplateMicroSummary>, IEnumerable<PageTemplateMicroSummary>>
        , IAsyncQueryHandler<GetAllQuery<PageTemplateMicroSummary>, IEnumerable<PageTemplateMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<PageTemplateMicroSummary>, IEnumerable<PageTemplateMicroSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetAllPageTemplateMicroSummariesQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public IEnumerable<PageTemplateMicroSummary> Execute(GetAllQuery<PageTemplateMicroSummary> query, IExecutionContext executionContext)
        {
            var dbResults = Query().ToList();
            var results = Map(dbResults);
            return results;
        }

        public async Task<IEnumerable<PageTemplateMicroSummary>> ExecuteAsync(GetAllQuery<PageTemplateMicroSummary> query, IExecutionContext executionContext)
        {
            var dbResults = await Query().ToListAsync();
            var results = Map(dbResults);
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
            return Mapper.Map<IEnumerable<PageTemplateMicroSummary>>(pageTemplates);
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
