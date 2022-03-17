using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class AddDocumentAssetCommandHandler
    : ICommandHandler<AddDocumentAssetCommand>
    , IPermissionRestrictedCommandHandler<AddDocumentAssetCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly EntityAuditHelper _entityAuditHelper;
    private readonly EntityTagHelper _entityTagHelper;
    private readonly DocumentAssetCommandHelper _documentAssetCommandHelper;
    private readonly ITransactionScopeManager _transactionScopeFactory;
    private readonly IMessageAggregator _messageAggregator;
    private readonly IRandomStringGenerator _randomStringGenerator;

    public AddDocumentAssetCommandHandler(
        CofoundryDbContext dbContext,
        EntityAuditHelper entityAuditHelper,
        EntityTagHelper entityTagHelper,
        DocumentAssetCommandHelper documentAssetCommandHelper,
        ITransactionScopeManager transactionScopeFactory,
        IMessageAggregator messageAggregator,
        IRandomStringGenerator randomStringGenerator
        )
    {
        _dbContext = dbContext;
        _entityAuditHelper = entityAuditHelper;
        _entityTagHelper = entityTagHelper;
        _documentAssetCommandHelper = documentAssetCommandHelper;
        _transactionScopeFactory = transactionScopeFactory;
        _messageAggregator = messageAggregator;
        _randomStringGenerator = randomStringGenerator;
    }

    public async Task ExecuteAsync(AddDocumentAssetCommand command, IExecutionContext executionContext)
    {
        var documentAsset = new DocumentAsset();

        documentAsset.Title = command.Title;
        documentAsset.Description = command.Description ?? string.Empty;
        documentAsset.FileName = FilePathHelper.CleanFileName(command.Title);
        documentAsset.VerificationToken = _randomStringGenerator.Generate(6);

        if (string.IsNullOrWhiteSpace(documentAsset.FileName))
        {
            throw ValidationErrorException.CreateWithProperties("Document title is empty or does not contain any safe file path characters.", nameof(command.Title));
        }

        _entityTagHelper.UpdateTags(documentAsset.DocumentAssetTags, command.Tags, executionContext);
        _entityAuditHelper.SetCreated(documentAsset, executionContext);
        documentAsset.FileUpdateDate = executionContext.ExecutionDate;

        using (var scope = _transactionScopeFactory.Create(_dbContext))
        {
            _dbContext.DocumentAssets.Add(documentAsset);

            await _documentAssetCommandHelper.SaveFile(command.File, documentAsset);

            command.OutputDocumentAssetId = documentAsset.DocumentAssetId;

            scope.QueueCompletionTask(() => OnTransactionComplete(documentAsset));

            await scope.CompleteAsync();
        }
    }

    private Task OnTransactionComplete(DocumentAsset documentAsset)
    {
        return _messageAggregator.PublishAsync(new DocumentAssetAddedMessage()
        {
            DocumentAssetId = documentAsset.DocumentAssetId
        });
    }

    public IEnumerable<IPermissionApplication> GetPermissions(AddDocumentAssetCommand command)
    {
        yield return new DocumentAssetCreatePermission();
    }
}