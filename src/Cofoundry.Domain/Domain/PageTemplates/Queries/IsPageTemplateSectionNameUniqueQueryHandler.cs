using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class IsPageTemplateSectionNameUniqueQueryHandler 
        : IAsyncQueryHandler<IsPageTemplateSectionNameUniqueQuery, bool>
        , IPermissionRestrictedQueryHandler<IsPageTemplateSectionNameUniqueQuery, bool>
    {
        private readonly CofoundryDbContext _dbContext;

        public IsPageTemplateSectionNameUniqueQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }


        public async Task<bool> ExecuteAsync(IsPageTemplateSectionNameUniqueQuery query, IExecutionContext executionContext)
        {
            var exists = await _dbContext
                .PageTemplateSections
                .AsNoTracking()
                .AnyAsync(d => d.PageTemplateSectionId != query.PageTemplateSectionId
                    && d.Name == query.Name
                    && d.PageTemplateId == query.PageTemplateId
                    );

            return !exists;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(IsPageTemplateSectionNameUniqueQuery query)
        {
            yield return new PageTemplateReadPermission();
        }

        #endregion
    }

}
