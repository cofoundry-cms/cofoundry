﻿using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Updates the draft version of a custom entity. If a draft version
/// does not exist then one is created first from the currently
/// published version.
/// </summary>
public class UpdatePageDraftVersionCommandHandler
    : ICommandHandler<UpdatePageDraftVersionCommand>
    , IPermissionRestrictedCommandHandler<UpdatePageDraftVersionCommand>
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly CofoundryDbContext _dbContext;
    private readonly IPageCache _pageCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly ITransactionScopeManager _transactionScopeFactory;
    private readonly IPageStoredProcedures _pageStoredProcedures;
    private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;

    public UpdatePageDraftVersionCommandHandler(
        ICommandExecutor commandExecutor,
        CofoundryDbContext dbContext,
        IPageCache pageCache,
        IMessageAggregator messageAggregator,
        ITransactionScopeManager transactionScopeFactory,
        IPageStoredProcedures pageStoredProcedures,
        IDbUnstructuredDataSerializer dbUnstructuredDataSerializer
        )
    {
        _commandExecutor = commandExecutor;
        _dbContext = dbContext;
        _pageCache = pageCache;
        _messageAggregator = messageAggregator;
        _transactionScopeFactory = transactionScopeFactory;
        _pageStoredProcedures = pageStoredProcedures;
        _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
    }

    public async Task ExecuteAsync(UpdatePageDraftVersionCommand command, IExecutionContext executionContext)
    {
        Normalize(command);

        var draft = await GetDraftVersionAsync(command.PageId);

        using (var scope = _transactionScopeFactory.Create(_dbContext))
        {
            draft = await CreateDraftIfRequiredAsync(command.PageId, draft);
            UpdateDraft(command, draft);

            await _dbContext.SaveChangesAsync();
            await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);

            var dependencyCommand = new UpdateUnstructuredDataDependenciesCommand(
                PageVersionEntityDefinition.DefinitionCode,
                draft.PageVersionId,
                command.ExtensionData
                );

            scope.QueueCompletionTask(() => OnTransactionComplete(draft));

            if (command.Publish)
            {
                await _commandExecutor.ExecuteAsync(new PublishPageCommand(draft.PageId, command.PublishDate), executionContext);
            }

            await scope.CompleteAsync();
        }
    }

    private Task OnTransactionComplete(PageVersion draft)
    {
        _pageCache.Clear(draft.PageId);

        return _messageAggregator.PublishAsync(new PageDraftVersionUpdatedMessage()
        {
            PageId = draft.PageId,
            PageVersionId = draft.PageVersionId
        });
    }

    private void Normalize(UpdatePageDraftVersionCommand command)
    {
        command.Title = command.Title.Trim();
        command.MetaDescription = command.MetaDescription?.Trim() ?? string.Empty;
        command.OpenGraphTitle = command.OpenGraphTitle?.Trim();
        command.OpenGraphDescription = command.OpenGraphDescription?.Trim();
    }

    private Task<PageVersion> GetDraftVersionAsync(int pageId)
    {
        return _dbContext
            .PageVersions
            .Where(p => p.PageId == pageId && p.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
            .SingleOrDefaultAsync();
    }

    private async Task<PageVersion> CreateDraftIfRequiredAsync(int pageId, PageVersion draft)
    {
        if (draft != null) return draft;

        var command = new AddPageDraftVersionCommand();
        command.PageId = pageId;
        await _commandExecutor.ExecuteAsync(command);

        return await GetDraftVersionAsync(pageId);
    }

    private void UpdateDraft(UpdatePageDraftVersionCommand command, PageVersion draft)
    {
        EntityNotFoundException.ThrowIfNull(draft, "Draft:" + command.PageId);

        draft.Title = command.Title;
        draft.ExcludeFromSitemap = !command.ShowInSiteMap;
        draft.MetaDescription = command.MetaDescription;
        draft.OpenGraphTitle = command.OpenGraphTitle;
        draft.OpenGraphDescription = command.OpenGraphDescription;
        draft.OpenGraphImageId = command.OpenGraphImageId;
        draft.SerializedExtensionData = _dbUnstructuredDataSerializer.Serialize(command.ExtensionData);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageDraftVersionCommand command)
    {
        yield return new PageUpdatePermission();

        if (command.Publish)
        {
            yield return new PagePublishPermission();
        }
    }
}
