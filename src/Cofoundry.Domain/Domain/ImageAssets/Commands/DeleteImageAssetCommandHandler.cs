using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Removes an image asset from the system and
/// queues any related files or caches to be removed
/// as a separate process.
/// </summary>
public class DeleteImageAssetCommandHandler
    : ICommandHandler<DeleteImageAssetCommand>
    , IPermissionRestrictedCommandHandler<DeleteImageAssetCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IImageAssetCache _imageAssetCache;
    private readonly ICommandExecutor _commandExecutor;
    private readonly ITransactionScopeManager _transactionScopeFactory;
    private readonly IMessageAggregator _messageAggregator;
    private readonly IDependableEntityDeleteCommandValidator _dependableEntityDeleteCommandValidator;

    public DeleteImageAssetCommandHandler(
        CofoundryDbContext dbContext,
        IImageAssetCache imageAssetCache,
        ICommandExecutor commandExecutor,
        ITransactionScopeManager transactionScopeFactory,
        IMessageAggregator messageAggregator,
        IDependableEntityDeleteCommandValidator dependableEntityDeleteCommandValidator
        )
    {
        _dbContext = dbContext;
        _imageAssetCache = imageAssetCache;
        _commandExecutor = commandExecutor;
        _transactionScopeFactory = transactionScopeFactory;
        _messageAggregator = messageAggregator;
        _dependableEntityDeleteCommandValidator = dependableEntityDeleteCommandValidator;
    }

    public async Task ExecuteAsync(DeleteImageAssetCommand command, IExecutionContext executionContext)
    {
        var imageAsset = await _dbContext
            .ImageAssets
            .FilterById(command.ImageAssetId)
            .SingleOrDefaultAsync();

        if (imageAsset != null)
        {
            await _dependableEntityDeleteCommandValidator.ValidateAsync(ImageAssetEntityDefinition.DefinitionCode, imageAsset.ImageAssetId, executionContext);

            var deleteFileCommand = new QueueAssetFileDeletionCommand()
            {
                EntityDefinitionCode = ImageAssetEntityDefinition.DefinitionCode,
                FileNameOnDisk = imageAsset.FileNameOnDisk,
                FileExtension = imageAsset.FileExtension
            };
            _dbContext.ImageAssets.Remove(imageAsset);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();
                await _commandExecutor.ExecuteAsync(deleteFileCommand, executionContext);

                scope.QueueCompletionTask(() => OnTransactionComplete(command));
                await scope.CompleteAsync();
            }
        }
    }

    private Task OnTransactionComplete(DeleteImageAssetCommand command)
    {
        _imageAssetCache.Clear(command.ImageAssetId);

        return _messageAggregator.PublishAsync(new ImageAssetDeletedMessage()
        {
            ImageAssetId = command.ImageAssetId
        });
    }

    public IEnumerable<IPermissionApplication> GetPermissions(DeleteImageAssetCommand command)
    {
        yield return new ImageAssetDeletePermission();
    }
}
