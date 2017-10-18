using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetUpdatePageDraftVersionCommandByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<UpdatePageDraftVersionCommand>, UpdatePageDraftVersionCommand>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<UpdatePageDraftVersionCommand>, UpdatePageDraftVersionCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetUpdatePageDraftVersionCommandByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public async Task<UpdatePageDraftVersionCommand> ExecuteAsync(GetByIdQuery<UpdatePageDraftVersionCommand> query, IExecutionContext executionContext)
        {
            var command = await _dbContext
                .PageVersions
                .AsNoTracking()
                .Where(p => p.PageId == query.Id && p.WorkFlowStatusId == (int)WorkFlowStatus.Draft && !p.IsDeleted)
                .Select(v => new UpdatePageDraftVersionCommand
                {
                    MetaDescription = v.MetaDescription,
                    OpenGraphDescription = v.OpenGraphDescription,
                    OpenGraphImageId = v.OpenGraphImageId,
                    OpenGraphTitle = v.OpenGraphTitle,
                    PageId = v.PageId,
                    ShowInSiteMap = !v.ExcludeFromSitemap,
                    Title = v.Title
                })
                .SingleOrDefaultAsync();

            return command;
        }

        #endregion
        
        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<UpdatePageDraftVersionCommand> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
