using Cofoundry.Core;
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
    /// Updates an existing page access rule.
    /// </summary>
    public class UpdatePageAccessRuleCommandHandler
        : ICommandHandler<UpdatePageAccessRuleCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageAccessRuleCommand>
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IPageCache _pageCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public UpdatePageAccessRuleCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IPageCache pageCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _dbContext = dbContext;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _pageCache = pageCache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task ExecuteAsync(UpdatePageAccessRuleCommand command, IExecutionContext executionContext)
        {
            var accessRule = await GetAccessRuleAsync(command.PageAccessRuleId);
            var userArea = await GetUserAreaAsync(command.UserAreaCode, accessRule, executionContext);
            var role = await GetRoleAsync(command, accessRule);

            ValidateRoleIsInUserArea(userArea, role);
            await ValidateIsUniqueAsync(command, accessRule, executionContext);

            UpdateAccessRule(accessRule, command, userArea, role);

            await _dbContext.SaveChangesAsync();
            await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(accessRule));
        }

        private Task OnTransactionComplete(PageAccessRule accessRule)
        {
            _pageCache.Clear(accessRule.PageId);

            return _messageAggregator.PublishAsync(new PageAccessRuleUpdatedMessage()
            {
                PageId = accessRule.PageId,
                PageAccessRuleId = accessRule.PageAccessRuleId,
            });
        }

        private async Task<PageAccessRule> GetAccessRuleAsync(int id)
        {
            var accessRule = await _dbContext
                .PageAccessRules
                .FilterById(id)
                .Include(r => r.Role)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(accessRule, id);

            return accessRule;
        }

        private async Task<IUserAreaDefinition> GetUserAreaAsync(
            string userAreaCode,
            PageAccessRule pageAccessRule,
            IExecutionContext executionContext
            )
        {
            var userArea = _userAreaDefinitionRepository.GetByCode(userAreaCode);

            if (userArea.UserAreaCode != pageAccessRule.UserAreaCode)
            {
                await _commandExecutor.ExecuteAsync(new EnsureUserAreaExistsCommand(userArea.UserAreaCode), executionContext);
            }

            return userArea;
        }

        private async Task<Role> GetRoleAsync(UpdatePageAccessRuleCommand command, PageAccessRule accessRule)
        {
            if (!command.RoleId.HasValue) return null;
            if (command.RoleId == accessRule.Role?.RoleId) return accessRule.Role;

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

        private async Task ValidateIsUniqueAsync(
            UpdatePageAccessRuleCommand command,
            PageAccessRule accessRule,
            IExecutionContext executionContext
            )
        {
            if (command.RoleId == accessRule.RoleId
                && command.UserAreaCode == accessRule.UserAreaCode)
            {
                return;
            }

            var isUnique = await _queryExecutor.ExecuteAsync(new IsPageAccessRuleUniqueQuery()
            {
                PageId = accessRule.PageId,
                PageAccessRuleId = accessRule.PageAccessRuleId,
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

        private void UpdateAccessRule(
            PageAccessRule accessRule,
            UpdatePageAccessRuleCommand command,
            IUserAreaDefinition userArea,
            Role role
            )
        {
            accessRule.UserAreaCode = userArea.UserAreaCode;
            accessRule.Role = role;
            accessRule.RouteAccessRuleViolationActionId = (int)command.ViolationAction;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageAccessRuleCommand command)
        {
            yield return new PageAccessRuleManagePermission();
        }
    }
}
