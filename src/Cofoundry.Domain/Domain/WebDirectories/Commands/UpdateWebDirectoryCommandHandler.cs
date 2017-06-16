using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class UpdateWebDirectoryCommandHandler 
        : IAsyncCommandHandler<UpdateWebDirectoryCommand>
        , IPermissionRestrictedCommandHandler<UpdateWebDirectoryCommand>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly IWebDirectoryCache _cache;

        public UpdateWebDirectoryCommandHandler(
            IQueryExecutor queryExecutor,
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            IWebDirectoryCache cache
            )
        {
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
            _queryExecutor = queryExecutor;
            _cache = cache;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(UpdateWebDirectoryCommand command, IExecutionContext executionContext)
        {
            var webDirectory = await _dbContext
                .WebDirectories
                .SingleOrDefaultAsync(w => w.WebDirectoryId == command.WebDirectoryId);
            EntityNotFoundException.ThrowIfNull(webDirectory, command.WebDirectoryId);

            // Custom Validation
            await ValidateIsUnique(command);
            await ValidateUrlPropertiesAllowedToChange(command, webDirectory);

            webDirectory.Name = command.Name;
            webDirectory.UrlPath = command.UrlPath;
            webDirectory.ParentWebDirectoryId = command.ParentWebDirectoryId;

            await _dbContext.SaveChangesAsync();

            _cache.Clear();
        }

        private async Task ValidateUrlPropertiesAllowedToChange(UpdateWebDirectoryCommand command, WebDirectory webDirectory)
        {
            var props = GetChangedPathProperties(command, webDirectory);

            if (props.Any() && await HasDependencies(webDirectory)) 
            {
                throw new PropertyValidationException("This directory is in use and the url cannot be changed", props.First());
            }
        }

        private IEnumerable<string> GetChangedPathProperties(UpdateWebDirectoryCommand command, WebDirectory webDirectory) 
        {
            if (command.UrlPath != webDirectory.UrlPath) yield return "UrlPath";
            if (command.ParentWebDirectoryId != webDirectory.ParentWebDirectoryId) yield return "ParentWebDirectoryId";
        }

        private async Task<bool> HasDependencies(WebDirectory webDirectory)
        {
            if (await _dbContext
                .WebDirectories
                .AsNoTracking()
                .Where(w => w.ParentWebDirectoryId == webDirectory.WebDirectoryId)
                .AnyAsync())
            {
                return true;
            }

            return await _dbContext
                .Pages
                .AsNoTracking()
                .Where(p => p.WebDirectoryId == webDirectory.WebDirectoryId)
                .AnyAsync();
        }

        private async Task ValidateIsUnique(UpdateWebDirectoryCommand command)
        {
            var query = new IsWebDirectoryPathUniqueQuery();
            query.ParentWebDirectoryId = command.ParentWebDirectoryId;
            query.UrlPath = command.UrlPath;
            query.WebDirectoryId = command.WebDirectoryId;

            var isUnique = await _queryExecutor.ExecuteAsync(query);

            if (!isUnique)
            {
                var message = string.Format("A web directory already exists in that parent directory with the path '{0}'", command.UrlPath);
                throw new UniqueConstraintViolationException(message, "UrlPath", command.UrlPath);
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(UpdateWebDirectoryCommand command)
        {
            yield return new WebDirectoryUpdatePermission();
        }

        #endregion
    }
}
