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
    public class DeletePageTemplateCommandHandler 
        : IAsyncCommandHandler<DeletePageTemplateCommand>
        , IPermissionRestrictedCommandHandler<DeletePageTemplateCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public DeletePageTemplateCommandHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        public async Task ExecuteAsync(DeletePageTemplateCommand command, IExecutionContext executionContext)
        {
            var template = await _dbContext
                .PageTemplates
                .SingleOrDefaultAsync(p => p.PageTemplateId == command.PageTemplateId);

            if (template != null)
            {
                _dbContext.PageTemplates.Remove(template);
                await _dbContext.SaveChangesAsync();
            }
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageTemplateCommand command)
        {
            yield return new PageTemplateDeletePermission();
        }

        #endregion
    }
}
