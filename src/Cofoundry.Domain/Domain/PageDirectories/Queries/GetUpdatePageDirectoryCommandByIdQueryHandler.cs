using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetUpdatePageDirectoryCommandByIdQueryHandler
        : IAsyncQueryHandler<GetByIdQuery<UpdatePageDirectoryCommand>, UpdatePageDirectoryCommand>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<UpdatePageDirectoryCommand>, UpdatePageDirectoryCommand>
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

        public async Task<UpdatePageDirectoryCommand> ExecuteAsync(GetByIdQuery<UpdatePageDirectoryCommand> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .PageDirectories
                .AsNoTracking()
                .Where(w => w.PageDirectoryId == query.Id && w.IsActive)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(dbResult, query.Id);

            var command = Mapper.Map<UpdatePageDirectoryCommand>(dbResult);
            return command;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<UpdatePageDirectoryCommand> command)
        {
            yield return new PageDirectoryReadPermission();
        }

        #endregion
    }
}
