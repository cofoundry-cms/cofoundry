using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Updates all access rules associated with a page.
/// </summary>
public class UpdatePageAccessRuleSetCommandHandler
    : ICommandHandler<UpdatePageAccessRuleSetCommand>
    , IPermissionRestrictedCommandHandler<UpdatePageAccessRuleSetCommand>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IDomainRepository _domainRepository;
    private readonly IUpdateAccessRuleSetCommandHelper _updateAccessRulesCommandHelper;
    private readonly IPageCache _pageCache;
    private readonly IMessageAggregator _messageAggregator;

    public UpdatePageAccessRuleSetCommandHandler(
        CofoundryDbContext dbContext,
        IDomainRepository domainRepository,
        IUpdateAccessRuleSetCommandHelper updateAccessRulesCommandHelper,
        IPageCache pageCache,
        IMessageAggregator messageAggregator
        )
    {
        _dbContext = dbContext;
        _domainRepository = domainRepository;
        _updateAccessRulesCommandHelper = updateAccessRulesCommandHelper;
        _pageCache = pageCache;
        _messageAggregator = messageAggregator;
    }

    public async Task ExecuteAsync(UpdatePageAccessRuleSetCommand command, IExecutionContext executionContext)
    {
        var page = await GetPageAsync(command);
        await _updateAccessRulesCommandHelper.UpdateAsync(page, command, executionContext);

        await _dbContext.SaveChangesAsync();
        await _domainRepository.Transactions().QueueCompletionTaskAsync(() => OnTransactionComplete(command));
    }

    private async Task<Page> GetPageAsync(UpdatePageAccessRuleSetCommand command)
    {
        var page = await _dbContext
            .Pages
            .Include(p => p.AccessRules)
            .FilterActive()
            .FilterById(command.PageId)
            .SingleOrDefaultAsync();

        EntityNotFoundException.ThrowIfNull(page, command.PageId);

        return page;
    }

    private Task OnTransactionComplete(UpdatePageAccessRuleSetCommand command)
    {
        _pageCache.Clear(command.PageId);

        return _messageAggregator.PublishAsync(new PageAccessRulesUpdatedMessage()
        {
            PageId = command.PageId
        });
    }

    public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageAccessRuleSetCommand command)
    {
        yield return new PageAccessRuleManagePermission();
    }
}
