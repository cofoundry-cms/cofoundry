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
    /// Removes an access rule from a page directory.
    /// </summary>
    public class DeletePageDirectoryAccessRuleCommandHandler
        : ICommandHandler<DeletePageDirectoryAccessRuleCommand>
        , IPermissionRestrictedCommandHandler<DeletePageDirectoryAccessRuleCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IPageDirectoryCache _pageDirectoryCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public DeletePageDirectoryAccessRuleCommandHandler(
            CofoundryDbContext dbContext,
            IPageDirectoryCache pageDirectoryCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _pageDirectoryCache = pageDirectoryCache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task ExecuteAsync(DeletePageDirectoryAccessRuleCommand command, IExecutionContext executionContext)
        {
            var accessRule = await _dbContext
                .PageDirectoryAccessRules
                .FilterById(command.PageDirectoryAccessRuleId)
                .SingleOrDefaultAsync();

            if (accessRule != null)
            {
                var pageDirectoryId = accessRule.PageDirectoryId;
                _dbContext.PageDirectoryAccessRules.Remove(accessRule);

                await _dbContext.SaveChangesAsync();
                await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(command, pageDirectoryId));
            }
        }

        private Task OnTransactionComplete(DeletePageDirectoryAccessRuleCommand command, int pageDirectoryId)
        {
            _pageDirectoryCache.Clear();

            return _messageAggregator.PublishAsync(new PageDirectoryAccessRuleDeletedMessage()
            {
                PageDirectoryId = pageDirectoryId,
                PageDirectoryAccessRuleId = command.PageDirectoryAccessRuleId
            });
        }

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageDirectoryAccessRuleCommand command)
        {
            yield return new PageDirectoryAccessRuleManagePermission();
        }
    }
}
