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
    /// Updates all access rules associated with a page directory.
    /// </summary>
    public class UpdatePageDirectoryAccessRuleSetCommandHandler
        : ICommandHandler<UpdatePageDirectoryAccessRuleSetCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageDirectoryAccessRuleSetCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUpdateAccessRuleSetCommandHelper _updateAccessRulesCommandHelper;
        private readonly IPageDirectoryCache _pageDirectoryCache;
        private readonly IMessageAggregator _messageAggregator;

        public UpdatePageDirectoryAccessRuleSetCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUpdateAccessRuleSetCommandHelper updateAccessRulesCommandHelper,
            IPageDirectoryCache pageDirectoryCache,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _updateAccessRulesCommandHelper = updateAccessRulesCommandHelper;
            _pageDirectoryCache = pageDirectoryCache;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(UpdatePageDirectoryAccessRuleSetCommand command, IExecutionContext executionContext)
        {
            var directory = await GetPageDirectoryAsync(command);
            await _updateAccessRulesCommandHelper.UpdateAsync(directory, command, executionContext);

            await _dbContext.SaveChangesAsync();
            await _domainRepository.Transactions().QueueCompletionTaskAsync(() => OnTransactionComplete(command));
        }

        private async Task<PageDirectory> GetPageDirectoryAsync(UpdatePageDirectoryAccessRuleSetCommand command)
        {
            var directory = await _dbContext
                .PageDirectories
                .Include(p => p.AccessRules)
                .FilterById(command.PageDirectoryId)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(directory, command.PageDirectoryId);

            return directory;
        }

        private Task OnTransactionComplete(UpdatePageDirectoryAccessRuleSetCommand command)
        {
            _pageDirectoryCache.Clear();

            return _messageAggregator.PublishAsync(new PageDirectoryAccessRulesUpdatedMessage()
            {
                PageDirectoryId = command.PageDirectoryId
            });
        }

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageDirectoryAccessRuleSetCommand command)
        {
            yield return new PageDirectoryAccessRuleManagePermission();
        }
    }
}
