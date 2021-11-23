using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class DeletePageCommandHandler
        : ICommandHandler<DeletePageCommand>
        , IPermissionRestrictedCommandHandler<DeletePageCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IPageStoredProcedures _pageStoredProcedures;
        private readonly IDependableEntityDeleteCommandValidator _dependableEntityDeleteCommandValidator;

        public DeletePageCommandHandler(
            CofoundryDbContext dbContext,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory,
            IPageStoredProcedures pageStoredProcedures,
            IDependableEntityDeleteCommandValidator dependableEntityDeleteCommandValidator
            )
        {
            _dbContext = dbContext;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
            _pageStoredProcedures = pageStoredProcedures;
            _dependableEntityDeleteCommandValidator = dependableEntityDeleteCommandValidator;
        }

        public async Task ExecuteAsync(DeletePageCommand command, IExecutionContext executionContext)
        {
            var page = await _dbContext
                .Pages
                .FilterById(command.PageId)
                .SingleOrDefaultAsync();

            if (page != null)
            {
                await _dependableEntityDeleteCommandValidator.ValidateAsync(PageEntityDefinition.DefinitionCode, command.PageId, executionContext);

                _dbContext.Pages.Remove(page);

                using (var scope = _transactionScopeFactory.Create(_dbContext))
                {
                    await _dbContext.SaveChangesAsync();
                    await _pageStoredProcedures.UpdatePublishStatusQueryLookupAsync(command.PageId);

                    scope.QueueCompletionTask(() => OnTransactionComplete(command));
                    await scope.CompleteAsync();
                }
            }
        }

        private Task OnTransactionComplete(DeletePageCommand command)
        {
            _pageCache.Clear(command.PageId);

            return _messageAggregator.PublishAsync(new PageDeletedMessage()
            {
                PageId = command.PageId
            });
        }

        public IEnumerable<IPermissionApplication> GetPermissions(DeletePageCommand command)
        {
            yield return new PageDeletePermission();
        }
    }
}
