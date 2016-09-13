using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class AddRootWebDirectoryCommandHandler 
        : IAsyncCommandHandler<AddRootWebDirectoryCommand>
        , IPermissionRestrictedCommandHandler<AddRootWebDirectoryCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly IWebDirectoryCache _cache;

        public AddRootWebDirectoryCommandHandler(
            CofoundryDbContext dbContext,
            EntityAuditHelper entityAuditHelper,
            IWebDirectoryCache cache
            )
        {
            _cache = cache;
            _dbContext = dbContext;
            _entityAuditHelper = entityAuditHelper;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(AddRootWebDirectoryCommand command, IExecutionContext executionContext)
        {
            var webDirectory = PreSave(executionContext);
            await _dbContext.SaveChangesAsync();
            PostSave(command, webDirectory);
        }

        #endregion

        #region helpers
        
        private WebDirectory PreSave(IExecutionContext executionContext)
        {

            var webDirectory = new WebDirectory();
            webDirectory.Name = "Root";
            webDirectory.UrlPath = string.Empty;
            webDirectory.IsActive = true;
            _entityAuditHelper.SetCreated(webDirectory, executionContext);

            _dbContext.WebDirectories.Add(webDirectory);
            return webDirectory;
        }

        private void PostSave(AddRootWebDirectoryCommand command, WebDirectory webDirectory)
        {
            _cache.Clear();
            command.OutputWebDirectoryId = webDirectory.WebDirectoryId;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddRootWebDirectoryCommand command)
        {
            yield return new WebDirectoryCreatePermission();
        }

        #endregion
    }
}
