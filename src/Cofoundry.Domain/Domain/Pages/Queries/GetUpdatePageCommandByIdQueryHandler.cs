using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class GetUpdatePageCommandByIdQueryHandler
        : IQueryHandler<GetUpdateCommandByIdQuery<UpdatePageCommand>, UpdatePageCommand>
        , IPermissionRestrictedQueryHandler<GetUpdateCommandByIdQuery<UpdatePageCommand>, UpdatePageCommand>
    {
        private readonly CofoundryDbContext _dbContext;

        public GetUpdatePageCommandByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<UpdatePageCommand> ExecuteAsync(GetUpdateCommandByIdQuery<UpdatePageCommand> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .Pages
                .AsNoTracking()
                .Include(p => p.PageTags)
                .ThenInclude(t => t.Tag)
                .FilterActive()
                .FilterById(query.Id)
                .SingleOrDefaultAsync();

            if (dbResult == null) return null;

            var command = new UpdatePageCommand()
            {
                PageId = dbResult.PageId,
                Tags = dbResult
                    .PageTags
                    .Select(t => t.Tag.TagText)
                    .OrderBy(t => t)
                    .ToArray()
            };

            return command;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetUpdateCommandByIdQuery<UpdatePageCommand> query)
        {
            yield return new PageReadPermission();
        }
    }
}
