using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class DeleteWebDirectoryCommandHandler 
        : IAsyncCommandHandler<DeleteWebDirectoryCommand>
        , IPermissionRestrictedCommandHandler<DeleteWebDirectoryCommand>
    {
        #region constructor 

        private readonly CofoundryDbContext _dbContext;
        private readonly IWebDirectoryCache _cache;

        public DeleteWebDirectoryCommandHandler(
            CofoundryDbContext dbContext,
            IWebDirectoryCache cache
            )
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(DeleteWebDirectoryCommand command, IExecutionContext executionContext)
        {
            var webDirectory = await _dbContext
                .WebDirectories
                .Include(w => w.ChildWebDirectories)
                .SingleOrDefaultAsync(w => w.WebDirectoryId == command.WebDirectoryId);
            EntityNotFoundException.ThrowIfNull(webDirectory, command.WebDirectoryId);

            if (!webDirectory.ParentWebDirectoryId.HasValue)
            {
                throw new ValidationException("Cannot delete the root web directory.");
            }
            webDirectory.IsActive = false;

            await _dbContext.SaveChangesAsync();

            _cache.Clear();
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeleteWebDirectoryCommand command)
        {
            yield return new WebDirectoryDeletePermission();
        }

        #endregion
    }
}
