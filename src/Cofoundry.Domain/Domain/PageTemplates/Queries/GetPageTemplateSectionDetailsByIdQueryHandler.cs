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
    public class GetPageTemplateSectionDetailsByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<PageTemplateSectionDetails>, PageTemplateSectionDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<PageTemplateSectionDetails>, PageTemplateSectionDetails>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly PageTemplateSectionMapper _pageTemplateSectionMapper;

        public GetPageTemplateSectionDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            PageTemplateSectionMapper pageTemplateSectionMapper
            )
        {
            _pageTemplateSectionMapper = pageTemplateSectionMapper;
            _dbContext = dbContext;
        }

        public async Task<PageTemplateSectionDetails> ExecuteAsync(GetByIdQuery<PageTemplateSectionDetails> query, IExecutionContext executionContext)
        {
            var dbSection = await _dbContext
                .PageTemplateSections
                .AsNoTracking()
                .SingleOrDefaultAsync(l => l.PageTemplateSectionId == query.Id);

            var section = Mapper.Map<PageTemplateSectionDetails>(dbSection);

            return section;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<PageTemplateSectionDetails> query)
        {
            yield return new PageTemplateReadPermission();
        }

        #endregion
    }
}
