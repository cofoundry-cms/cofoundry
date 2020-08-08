using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public class GetUpdatePageDirectoryCommandByIdQueryHandler
        : IQueryHandler<GetUpdateCommandByIdQuery<UpdatePageDirectoryCommand>, UpdatePageDirectoryCommand>
        , IPermissionRestrictedQueryHandler<GetUpdateCommandByIdQuery<UpdatePageDirectoryCommand>, UpdatePageDirectoryCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetUpdatePageDirectoryCommandByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public async Task<UpdatePageDirectoryCommand> ExecuteAsync(GetUpdateCommandByIdQuery<UpdatePageDirectoryCommand> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .PageDirectories
                .AsNoTracking()
                .Where(w => w.PageDirectoryId == query.Id)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(dbResult, query.Id);

            if (!dbResult.ParentPageDirectoryId.HasValue)
            {
                throw new NotPermittedException("The root directory cannot be updated.");
            }

            var command = new UpdatePageDirectoryCommand()
            {
                Name = dbResult.Name,
                PageDirectoryId = dbResult.PageDirectoryId,
                ParentPageDirectoryId = dbResult.ParentPageDirectoryId.Value,
                UrlPath = dbResult.UrlPath
            };

            return command;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetUpdateCommandByIdQuery<UpdatePageDirectoryCommand> command)
        {
            yield return new PageDirectoryReadPermission();
        }

        #endregion
    }
}
