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
    public class GetUpdateWebDirectoryCommandByIdQueryHandler 
        : IAsyncQueryHandler<GetByIdQuery<UpdateWebDirectoryCommand>, UpdateWebDirectoryCommand>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<UpdateWebDirectoryCommand>, UpdateWebDirectoryCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetUpdateWebDirectoryCommandByIdQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public async Task<UpdateWebDirectoryCommand> ExecuteAsync(GetByIdQuery<UpdateWebDirectoryCommand> query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .WebDirectories
                .AsNoTracking()
                .Where(w => w.WebDirectoryId == query.Id && w.IsActive)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(dbResult, query.Id);

            var command = Mapper.Map<UpdateWebDirectoryCommand>(dbResult);
            return command;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<UpdateWebDirectoryCommand> command)
        {
            yield return new WebDirectoryReadPermission();
        }

        #endregion
    }
}
