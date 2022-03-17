using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class UpdatePageCommandHandler
    : ICommandHandler<UpdatePageCommand>
    , IPermissionRestrictedCommandHandler<UpdatePageCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly EntityTagHelper _entityTagHelper;
    private readonly IPageCache _pageCache;
    private readonly IMessageAggregator _messageAggregator;
    private readonly ITransactionScopeManager _transactionScopeFactory;

    public UpdatePageCommandHandler(
        CofoundryDbContext dbContext,
        EntityTagHelper entityTagHelper,
        IPageCache pageCache,
        IMessageAggregator messageAggregator,
        ITransactionScopeManager transactionScopeFactory
        )
    {
        _dbContext = dbContext;
        _entityTagHelper = entityTagHelper;
        _pageCache = pageCache;
        _messageAggregator = messageAggregator;
        _transactionScopeFactory = transactionScopeFactory;
    }

    public async Task ExecuteAsync(UpdatePageCommand command, IExecutionContext executionContext)
    {
        var page = await GetPageAsync(command.PageId);
        MapPage(command, executionContext, page);

        await _dbContext.SaveChangesAsync();
        await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(page));
    }

    private Task OnTransactionComplete(Page page)
    {
        _pageCache.Clear(page.PageId);

        return _messageAggregator.PublishAsync(new PageUpdatedMessage()
        {
            PageId = page.PageId,
            HasPublishedVersionChanged = page.PublishStatusCode == PublishStatusCode.Published
        });
    }

    private async Task<Page> GetPageAsync(int id)
    {
        var page = await _dbContext
            .Pages
            .Include(p => p.PageTags)
            .ThenInclude(a => a.Tag)
            .FilterActive()
            .FilterById(id)
            .SingleOrDefaultAsync();

        EntityNotFoundException.ThrowIfNull(page, id);

        return page;
    }

    private void MapPage(UpdatePageCommand command, IExecutionContext executionContext, Page page)
    {
        _entityTagHelper.UpdateTags(page.PageTags, command.Tags, executionContext);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageCommand command)
    {
        yield return new PageUpdatePermission();
    }
}
