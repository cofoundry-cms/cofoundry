using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.EntityFramework;
using Cofoundry.Core.MessageAggregator;

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
        private readonly IMessageAggregator _messageAggregator;

        public DeleteImageAssetCommandHandler(
            CofoundryDbContext dbContext,
            IImageAssetCache imageAssetCache,
            ICommandExecutor commandExecutor,
            ITransactionScopeFactory transactionScopeFactory,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _imageAssetCache = imageAssetCache;
            _commandExecutor = commandExecutor;
            _transactionScopeFactory = transactionScopeFactory;
            _messageAggregator = messageAggregator;
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
                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _commandExecutor.ExecuteAsync(new DeleteUnstructuredDataDependenciesCommand(ImageAssetEntityDefinition.DefinitionCode, imageAsset.ImageAssetId));

                    await _dbContext.SaveChangesAsync();
                    scope.Complete();
                }
                _imageAssetCache.Clear(command.ImageAssetId);

                await _messageAggregator.PublishAsync(new ImageAssetDeletedMessage()
                {
                    ImageAssetId = command.ImageAssetId
                });
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
