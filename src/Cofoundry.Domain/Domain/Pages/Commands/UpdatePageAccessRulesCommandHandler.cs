using Cofoundry.Core;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Updates all access rules associated with a page.
    /// </summary>
    public class UpdatePageAccessRulesCommandHandler
        : ICommandHandler<UpdatePageAccessRulesCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageAccessRulesCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUpdateAccessRulesCommandHelper _updateAccessRulesCommandHelper;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;

        public UpdatePageAccessRulesCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUpdateAccessRulesCommandHelper updateAccessRulesCommandHelper,
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

        public async Task ExecuteAsync(UpdatePageAccessRulesCommand command, IExecutionContext executionContext)
        {
            var page = await GetPageAsync(command);
            await _updateAccessRulesCommandHelper.UpdateAsync(page, command, executionContext);

            await _dbContext.SaveChangesAsync();
            await _domainRepository.Transactions().QueueCompletionTaskAsync(() => OnTransactionComplete(command));
        }

        private async Task<Page> GetPageAsync(UpdatePageAccessRulesCommand command)
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

        private Task OnTransactionComplete(UpdatePageAccessRulesCommand command)
        {
            _pageCache.Clear(command.PageId);

            return _messageAggregator.PublishAsync(new PageAccessRulesUpdatedMessage()
            {
                PageId = command.PageId
            });
        }

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageAccessRulesCommand command)
        {
            yield return new PageAccessRuleManagePermission();
        }
    }
}
