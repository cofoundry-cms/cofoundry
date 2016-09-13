using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;

namespace Cofoundry.Domain
{
    public class DeletePageTemplateSectionCommandHandler 
        : IAsyncCommandHandler<DeletePageTemplateSectionCommand>
        , IPermissionRestrictedCommandHandler<DeletePageTemplateSectionCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;

        public DeletePageTemplateSectionCommandHandler(
            CofoundryDbContext dbContext,
            IPageCache pageCache
            )
        {
            _dbContext = dbContext;
            _pageCache = pageCache;
        }

        #endregion

        public async Task ExecuteAsync(DeletePageTemplateSectionCommand command, IExecutionContext executionContext)
        {
            var templateSection = await _dbContext
                .PageTemplateSections
                .SingleOrDefaultAsync(p => p.PageTemplateSectionId == command.PageTemplateSectionId);

            if (templateSection != null)
            {
                _dbContext.PageTemplateSections.Remove(templateSection);
                await _dbContext.SaveChangesAsync();

                _pageCache.Clear();
            }
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageTemplateSectionCommand command)
        {
            yield return new PageTemplateUpdatePermission();
        }

        #endregion
    }
}
