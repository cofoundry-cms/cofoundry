using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class GetUpdatePageDraftVersionCommandByIdQueryHandler 
        : IQueryHandler<GetUpdateCommandByIdQuery<UpdatePageDraftVersionCommand>, UpdatePageDraftVersionCommand>
        , IPermissionRestrictedQueryHandler<GetUpdateCommandByIdQuery<UpdatePageDraftVersionCommand>, UpdatePageDraftVersionCommand>
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

        public async Task<UpdatePageDraftVersionCommand> ExecuteAsync(GetUpdateCommandByIdQuery<UpdatePageDraftVersionCommand> query, IExecutionContext executionContext)
        {
            var command = await _dbContext
                .PageVersions
                .AsNoTracking()
                .FilterActive()
                .FilterByPageId(query.Id)
                .Where(p => p.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
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

        public IEnumerable<IPermissionApplication> GetPermissions(GetUpdateCommandByIdQuery<UpdatePageDraftVersionCommand> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
