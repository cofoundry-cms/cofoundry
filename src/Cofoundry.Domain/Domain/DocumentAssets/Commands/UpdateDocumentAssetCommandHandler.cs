using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class UpdateDocumentAssetCommandHandler
    : ICommandHandler<UpdateDocumentAssetCommand>
    , IPermissionRestrictedCommandHandler<UpdateDocumentAssetCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly EntityAuditHelper _entityAuditHelper;
    private readonly EntityTagHelper _entityTagHelper;
    private readonly DocumentAssetCommandHelper _documentAssetCommandHelper;
    private readonly ITransactionScopeManager _transactionScopeFactory;
    private readonly IMessageAggregator _messageAggregator;
    private readonly ICommandExecutor _commandExecutor;

    public UpdateDocumentAssetCommandHandler(
        CofoundryDbContext dbContext,
        EntityAuditHelper entityAuditHelper,
        EntityTagHelper entityTagHelper,
        DocumentAssetCommandHelper documentAssetCommandHelper,
        ITransactionScopeManager transactionScopeFactory,
        IMessageAggregator messageAggregator,
        ICommandExecutor commandExecutor
        )
    {
        _dbContext = dbContext;
        _entityAuditHelper = entityAuditHelper;
        _entityTagHelper = entityTagHelper;
        _documentAssetCommandHelper = documentAssetCommandHelper;
        _transactionScopeFactory = transactionScopeFactory;
        _messageAggregator = messageAggregator;
        _commandExecutor = commandExecutor;
    }

    public async Task ExecuteAsync(UpdateDocumentAssetCommand command, IExecutionContext executionContext)
    {
        var documentAsset = await _dbContext
            .DocumentAssets
            .Include(a => a.DocumentAssetTags)
            .ThenInclude(a => a.Tag)
            .FilterById(command.DocumentAssetId)
            .SingleOrDefaultAsync();
        EntityNotFoundException.ThrowIfNull(documentAsset, command.DocumentAssetId);

        documentAsset.Title = command.Title;
        documentAsset.Description = command.Description ?? string.Empty;
        documentAsset.FileName = FilePathHelper.CleanFileName(command.Title);

        if (string.IsNullOrWhiteSpace(documentAsset.FileName))
        {
            throw ValidationErrorException.CreateWithProperties("Document title is empty or does not contain any safe file path characters.", nameof(command.Title));
        }

        _entityTagHelper.UpdateTags(documentAsset.DocumentAssetTags, command.Tags, executionContext);
        _entityAuditHelper.SetUpdated(documentAsset, executionContext);

        using (var scope = _transactionScopeFactory.Create(_dbContext))
        {
            bool hasNewFile = false;
            if (command.File != null)
            {
                var deleteOldFileCommand = new QueueAssetFileDeletionCommand()
                {
                    EntityDefinitionCode = DocumentAssetEntityDefinition.DefinitionCode,
                    FileNameOnDisk = documentAsset.FileNameOnDisk,
                    FileExtension = documentAsset.FileExtension
                };

                await _commandExecutor.ExecuteAsync(deleteOldFileCommand, executionContext);
                await _documentAssetCommandHelper.SaveFile(command.File, documentAsset);
                documentAsset.FileUpdateDate = executionContext.ExecutionDate;
                hasNewFile = true;
            }

            await _dbContext.SaveChangesAsync();

            scope.QueueCompletionTask(() => OnTransactionComplete(documentAsset, hasNewFile));

            await scope.CompleteAsync();
        }
    }

    private Task OnTransactionComplete(DocumentAsset documentAsset, bool hasNewFile)
    {
        return _messageAggregator.PublishAsync(new DocumentAssetUpdatedMessage()
        {
            DocumentAssetId = documentAsset.DocumentAssetId,
            HasFileChanged = hasNewFile
        });
    }

    public IEnumerable<IPermissionApplication> GetPermissions(UpdateDocumentAssetCommand command)
    {
        yield return new DocumentAssetUpdatePermission();
    }
}