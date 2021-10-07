using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Adds a new access rule to a page.
    /// </summary>
    public class AddPageAccessRuleCommandHandler
        : ICommandHandler<AddPageAccessRuleCommand>
        , IPermissionRestrictedCommandHandler<AddPageAccessRuleCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public AddPageAccessRuleCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            EntityAuditHelper entityAuditHelper,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _entityAuditHelper = entityAuditHelper;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task ExecuteAsync(AddPageAccessRuleCommand command, IExecutionContext executionContext)
        {
            var page = await GetPageAsync(command);
            var userArea = await GetUserAreaAsync(command.UserAreaCode, executionContext);
            var role = await GetRoleAsync(command);

            ValidateRoleIsInUserArea(userArea, role);
            await ValidateIsUniqueAsync(command, executionContext);

            var accessRule = MapAccessRule(command, page, userArea, role, executionContext);
            _dbContext.PageAccessRules.Add(accessRule);

            await _dbContext.SaveChangesAsync();
            await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(accessRule));

            command.OutputPageAccessRuleId = accessRule.PageAccessRuleId;
        }

        private async Task<Page> GetPageAsync(AddPageAccessRuleCommand command)
        {
            var page = await _dbContext
                .Pages
                .FilterActive()
                .FilterById(command.PageId)
                .SingleOrDefaultAsync();

            if (page == null)
            {
                throw ValidationErrorException.CreateWithProperties("Page does not exist.", nameof(command.PageId));
            }

            return page;
        }

        private async Task<IUserAreaDefinition> GetUserAreaAsync(string userAreaCode, IExecutionContext executionContext)
        {
            var userArea = _userAreaDefinitionRepository.GetByCode(userAreaCode);
            await _commandExecutor.ExecuteAsync(new EnsureUserAreaExistsCommand(userArea.UserAreaCode), executionContext);

            return userArea;
        }

        private async Task<Role> GetRoleAsync(AddPageAccessRuleCommand command)
        {
            if (!command.RoleId.HasValue) return null;

            var role = await _dbContext
                .Roles
                .FilterById(command.RoleId.Value)
                .SingleOrDefaultAsync();

            if (role == null)
            {
                throw ValidationErrorException.CreateWithProperties("Role does not exist.", nameof(command.RoleId));
            }

            return role;
        }

        private void ValidateRoleIsInUserArea(IUserAreaDefinition userAreaDefinition, Role role)
        {
            if (role != null && role.UserAreaCode != userAreaDefinition.UserAreaCode)
            {
                throw ValidationErrorException.CreateWithProperties($"This role is not in the {userAreaDefinition.Name} user area.", nameof(role.RoleId));
            }
        }

        private async Task ValidateIsUniqueAsync(AddPageAccessRuleCommand command, IExecutionContext executionContext)
        {
            var isUnique = await _queryExecutor.ExecuteAsync(new IsPageAccessRuleUniqueQuery()
            {
                PageId = command.PageId,
                UserAreaCode = command.UserAreaCode,
                RoleId = command.RoleId
            }, executionContext);

            if (!isUnique)
            {
                var propertyName = command.RoleId.HasValue ? nameof(command.RoleId) : nameof(command.UserAreaCode);
                var displayName = command.RoleId.HasValue ? "role" : "user area";
                var message = $"An existing access rule is already configured with this {displayName}.";
                throw new UniqueConstraintViolationException<PageAccessRule>(message, propertyName);
            }
        }


        private PageAccessRule MapAccessRule(
            AddPageAccessRuleCommand command,
            Page page,
            IUserAreaDefinition userArea,
            Role role,
            IExecutionContext executionContext
            )
        {
            var accessRule = new PageAccessRule()
            {
                Page = page,
                UserAreaCode = userArea.UserAreaCode,
                Role = role,
                RouteAccessRuleViolationActionId = (int)command.ViolationAction
            };

            _entityAuditHelper.SetCreated(accessRule, executionContext);

            return accessRule;
        }

        private Task OnTransactionComplete(PageAccessRule accessRule)
        {
            _pageCache.Clear(accessRule.PageId);

            return _messageAggregator.PublishAsync(new PageAccessRuleAddedMessage()
            {
                PageId = accessRule.PageId,
                PageAccessRuleId = accessRule.PageAccessRuleId
            });
        }

        public IEnumerable<IPermissionApplication> GetPermissions(AddPageAccessRuleCommand command)
        {
            yield return new PageAccessRuleManagePermission();
        }
    }
}
