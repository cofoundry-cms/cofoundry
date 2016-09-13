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
    public class GetUpdatePageTemplateCommandByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<UpdatePageTemplateCommand>, UpdatePageTemplateCommand>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<UpdatePageTemplateCommand>, UpdatePageTemplateCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetUpdatePageTemplateCommandByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        public async Task<UpdatePageTemplateCommand> ExecuteAsync(GetByIdQuery<UpdatePageTemplateCommand> query, IExecutionContext executionContext)
        {
            var command = await _dbContext
                .PageTemplates
                .AsNoTracking()
                .Where(l => l.PageTemplateId == query.Id)
                .ProjectTo<UpdatePageTemplateCommand>()
                .SingleOrDefaultAsync();

            return command;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<UpdatePageTemplateCommand> query)
        {
            yield return new PageTemplateReadPermission();
        }

        #endregion
    }
}
