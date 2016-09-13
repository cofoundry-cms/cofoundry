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
    public class IsPageTemplateNameUniqueQueryHandler 
        : IAsyncQueryHandler<IsPageTemplateNameUniqueQuery, bool>
        , IPermissionRestrictedQueryHandler<IsPageTemplateNameUniqueQuery, bool>
    {
        private readonly CofoundryDbContext _dbContext;

        public IsPageTemplateNameUniqueQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExecuteAsync(IsPageTemplateNameUniqueQuery query, IExecutionContext executionContext)
        {
            var exists = await _dbContext
                .PageTemplates
                .AnyAsync(d => d.PageTemplateId != query.PageTemplateId
                    && d.Name == query.Name
                    );

            return !exists;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(IsPageTemplateNameUniqueQuery query)
        {
            yield return new PageTemplateReadPermission();
        }

        #endregion
    }

}
