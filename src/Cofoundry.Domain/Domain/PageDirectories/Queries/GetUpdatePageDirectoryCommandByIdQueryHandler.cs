using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class GetUpdatePageDirectoryCommandByIdQueryHandler
        : IQueryHandler<GetUpdateCommandByIdQuery<UpdatePageDirectoryCommand>, UpdatePageDirectoryCommand>
        , IPermissionRestrictedQueryHandler<GetUpdateCommandByIdQuery<UpdatePageDirectoryCommand>, UpdatePageDirectoryCommand>
    {
        private readonly CofoundryDbContext _dbContext;

        public GetUpdatePageDirectoryCommandByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<UpdatePageDirectoryCommand> ExecuteAsync(GetUpdateCommandByIdQuery<UpdatePageDirectoryCommand> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .PageDirectories
                .AsNoTracking()
                .FilterById(query.Id)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(dbResult, query.Id);

            if (!dbResult.ParentPageDirectoryId.HasValue)
            {
                throw new NotPermittedException("The root directory cannot be updated.");
            }

            var command = new UpdatePageDirectoryCommand()
            {
                Name = dbResult.Name,
                PageDirectoryId = dbResult.PageDirectoryId
            };

            return command;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetUpdateCommandByIdQuery<UpdatePageDirectoryCommand> command)
        {
            yield return new PageDirectoryReadPermission();
        }
    }
}
