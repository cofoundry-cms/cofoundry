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
    public class GetPageTemplateSectionsByPageTemplateIdQueryHandler 
        : IAsyncQueryHandler<GetPageTemplateSectionsByPageTemplateIdQuery, IEnumerable<PageTemplateSectionDetails>>
        , IPermissionRestrictedQueryHandler<GetPageTemplateSectionsByPageTemplateIdQuery, IEnumerable<PageTemplateSectionDetails>>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly PageTemplateSectionMapper _pageTemplateSectionMapper;

        public GetPageTemplateSectionsByPageTemplateIdQueryHandler(
            CofoundryDbContext dbContext,
            PageTemplateSectionMapper pageTemplateSectionMapper
            )
        {
            _pageTemplateSectionMapper = pageTemplateSectionMapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PageTemplateSectionDetails>> ExecuteAsync(GetPageTemplateSectionsByPageTemplateIdQuery query, IExecutionContext executionContext)
        {
            var dbSections = await _dbContext
                .PageTemplateSections
                .AsNoTracking()
                .Include(l => l.PageModuleTypes)
                .Where(l => l.PageTemplateId == query.PageTemplateId)
                .ToListAsync();

            var sections = _pageTemplateSectionMapper.MapDetails(dbSections);

            return sections;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageTemplateSectionsByPageTemplateIdQuery query)
        {
            yield return new PageTemplateReadPermission();
        }

        #endregion
    }
}
