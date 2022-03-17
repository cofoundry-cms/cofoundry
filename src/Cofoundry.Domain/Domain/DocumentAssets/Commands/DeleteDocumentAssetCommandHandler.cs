using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class DeleteDocumentAssetCommandHandler
    : ICommandHandler<DeleteDocumentAssetCommand>
    , IPermissionRestrictedCommandHandler<DeleteDocumentAssetCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly ICommandExecutor _commandExecutor;
    private readonly ITransactionScopeManager _transactionScopeFactory;
    private readonly IMessageAggregator _messageAggregator;
    private readonly IDependableEntityDeleteCommandValidator _dependableEntityDeleteCommandValidator;

    public DeleteDocumentAssetCommandHandler(
        CofoundryDbContext dbContext,
        ICommandExecutor commandExecutor,
        ITransactionScopeManager transactionScopeFactory,
        IMessageAggregator messageAggregator,
        IDependableEntityDeleteCommandValidator dependableEntityDeleteCommandValidator
        )
    {
        _dbContext = dbContext;
        _commandExecutor = commandExecutor;
        _transactionScopeFactory = transactionScopeFactory;
        _messageAggregator = messageAggregator;
        _dependableEntityDeleteCommandValidator = dependableEntityDeleteCommandValidator;
    }

    public async Task ExecuteAsync(DeleteDocumentAssetCommand command, IExecutionContext executionContext)
    {
        var documentAsset = await _dbContext
            .DocumentAssets
            .FilterById(command.DocumentAssetId)
            .SingleOrDefaultAsync();

        if (documentAsset != null)
        {
            await _dependableEntityDeleteCommandValidator.ValidateAsync(DocumentAssetEntityDefinition.DefinitionCode, documentAsset.DocumentAssetId, executionContext);

            var deleteFileCommand = new QueueAssetFileDeletionCommand()
            {
                EntityDefinitionCode = DocumentAssetEntityDefinition.DefinitionCode,
                FileNameOnDisk = documentAsset.FileNameOnDisk,
                FileExtension = documentAsset.FileExtension
            };
            _dbContext.DocumentAssets.Remove(documentAsset);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                await _dbContext.SaveChangesAsync();
                await _commandExecutor.ExecuteAsync(deleteFileCommand, executionContext);

                scope.QueueCompletionTask(() => OnTransactionComplete(command));
                await scope.CompleteAsync();
            }
        }
    }

    private Task OnTransactionComplete(DeleteDocumentAssetCommand command)
    {
        return _messageAggregator.PublishAsync(new DocumentAssetAddedMessage()
        {
            DocumentAssetId = command.DocumentAssetId
        });
    }

    public IEnumerable<IPermissionApplication> GetPermissions(DeleteDocumentAssetCommand command)
    {
        yield return new DocumentAssetDeletePermission();
    }
}
