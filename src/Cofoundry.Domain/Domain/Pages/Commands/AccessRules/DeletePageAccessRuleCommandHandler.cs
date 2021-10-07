using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Removes an access rule from a page.
    /// </summary>
    public class DeletePageAccessRuleCommandHandler
        : ICommandHandler<DeletePageAccessRuleCommand>
        , IPermissionRestrictedCommandHandler<DeletePageAccessRuleCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public DeletePageAccessRuleCommandHandler(
            CofoundryDbContext dbContext,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task ExecuteAsync(DeletePageAccessRuleCommand command, IExecutionContext executionContext)
        {
            var accessRule = await _dbContext
                .PageAccessRules
                .FilterById(command.PageAccessRuleId)
                .SingleOrDefaultAsync();

            if (accessRule != null)
            {
                var pageId = accessRule.PageId;
                _dbContext.PageAccessRules.Remove(accessRule);

                await _dbContext.SaveChangesAsync();
                await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(command, pageId));
            }
        }

        private Task OnTransactionComplete(DeletePageAccessRuleCommand command, int pageId)
        {
            _pageCache.Clear(pageId);

            return _messageAggregator.PublishAsync(new PageAccessRuleDeletedMessage()
            {
                PageId = pageId,
                PageAccessRuleId = command.PageAccessRuleId
            });
        }

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageAccessRuleCommand command)
        {
            yield return new PageAccessRuleManagePermission();
        }
    }
}
