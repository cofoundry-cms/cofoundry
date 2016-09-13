using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain
{
    public class DeleteImageAssetCommandHandler 
        : IAsyncCommandHandler<DeleteImageAssetCommand>
        , IPermissionRestrictedCommandHandler<DeleteImageAssetCommand>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IImageAssetCache _imageAssetCache;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public DeleteImageAssetCommandHandler(
            CofoundryDbContext dbContext,
            IImageAssetCache imageAssetCache,
            ICommandExecutor commandExecutor,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _imageAssetCache = imageAssetCache;
            _commandExecutor = commandExecutor;
            _transactionScopeFactory = transactionScopeFactory;
        }

        #endregion

        #region Execute

        public async Task ExecuteAsync(DeleteImageAssetCommand command, IExecutionContext executionContext)
        {
            var imageAsset = await _dbContext
                .ImageAssets
                .FilterById(command.ImageAssetId)
                .SingleOrDefaultAsync();

            if (imageAsset != null)
            {
                imageAsset.IsDeleted = true;
                using (var scope = _transactionScopeFactory.Create())
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(ImageAssetEntityDefinition.DefinitionCode, imageAsset.ImageAssetId));

                    await _dbContext.SaveChangesAsync();
                    scope.Complete();
                }
                _imageAssetCache.Clear(imageAsset.ImageAssetId);
            }
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(DeleteImageAssetCommand command)
        {
            yield return new ImageAssetDeletePermission();
        }

        #endregion
    }
}
