using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using System.Data.Entity;

namespace Cofoundry.Domain
{
    public class GetUpdatePageTemplateSectionCommandByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<UpdatePageTemplateSectionCommand>, UpdatePageTemplateSectionCommand>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<UpdatePageTemplateSectionCommand>, UpdatePageTemplateSectionCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetUpdatePageTemplateSectionCommandByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        public async Task<UpdatePageTemplateSectionCommand> ExecuteAsync(GetByIdQuery<UpdatePageTemplateSectionCommand> query, IExecutionContext executionContext)
        {
            var command = await _dbContext
                .PageTemplateSections
                .AsNoTracking()
                .Where(l => l.PageTemplateSectionId == query.Id)
                .ProjectTo<UpdatePageTemplateSectionCommand>()
                .SingleOrDefaultAsync();

            return command;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<UpdatePageTemplateSectionCommand> query)
        {
            yield return new PageTemplateUpdatePermission();
        }

        #endregion
    }
}
