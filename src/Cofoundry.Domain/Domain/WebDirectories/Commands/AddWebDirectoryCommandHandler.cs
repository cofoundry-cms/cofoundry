using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class AddWebDirectoryCommandHandler 
        : IAsyncCommandHandler<AddWebDirectoryCommand>
        , IPermissionRestrictedCommandHandler<AddWebDirectoryCommand>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly IWebDirectoryCache _cache;

        public AddWebDirectoryCommandHandler(
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

        public async Task ExecuteAsync(AddWebDirectoryCommand command, IExecutionContext executionContext)
        {
            // Custom Validation
            await ValidateIsUnique(command);

            var webDirectory = new WebDirectory();
            webDirectory.Name = command.Name;
            webDirectory.UrlPath = command.UrlPath;
            webDirectory.IsActive = true;
            webDirectory.ParentWebDirectoryId = command.ParentWebDirectoryId;
            _entityAuditHelper.SetCreated(webDirectory, executionContext);

            _dbContext.WebDirectories.Add(webDirectory);
            await _dbContext.SaveChangesAsync();

            _cache.Clear();

            command.OutputWebDirectoryId = webDirectory.WebDirectoryId;
        }

        private async Task ValidateIsUnique(AddWebDirectoryCommand command)
        {
            var query = new IsWebDirectoryPathUniqueQuery();
            query.ParentWebDirectoryId = command.ParentWebDirectoryId;
            query.UrlPath = command.UrlPath;

            var isUnique = await _queryExecutor.ExecuteAsync(query);

            if (!isUnique)
            {
                var message = string.Format("A web directory already exists in that parent directory with the path '{0}'", command.UrlPath);
                throw new UniqueConstraintViolationException(message, "UrlPath", command.UrlPath);
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddWebDirectoryCommand command)
        {
            yield return new WebDirectoryCreatePermission();
        }

        #endregion
    }
}
