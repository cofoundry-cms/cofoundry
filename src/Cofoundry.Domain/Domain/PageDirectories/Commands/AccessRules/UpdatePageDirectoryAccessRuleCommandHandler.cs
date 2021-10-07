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
    /// Updates an existing page directory access rule.
    /// </summary>
    public class UpdatePageDirectoryAccessRuleCommandHandler
        : ICommandHandler<UpdatePageDirectoryAccessRuleCommand>
        , IPermissionRestrictedCommandHandler<UpdatePageDirectoryAccessRuleCommand>
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly CofoundryDbContext _dbContext;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IPageDirectoryCache _pageDirectoryCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public UpdatePageDirectoryAccessRuleCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IPageDirectoryCache pageDirectoryCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _dbContext = dbContext;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _pageDirectoryCache = pageDirectoryCache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task ExecuteAsync(UpdatePageDirectoryAccessRuleCommand command, IExecutionContext executionContext)
        {
            var accessRule = await GetAccessRuleAsync(command.PageDirectoryAccessRuleId);
            var userArea = await GetUserAreaAsync(command.UserAreaCode, accessRule, executionContext);
            var role = await GetRoleAsync(command, accessRule);

            ValidateRoleIsInUserArea(userArea, role);
            await ValidateIsUniqueAsync(command, accessRule, executionContext);

            UpdateAccessRule(accessRule, command, userArea, role);

            await _dbContext.SaveChangesAsync();
            await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(accessRule));
        }

        private Task OnTransactionComplete(PageDirectoryAccessRule accessRule)
        {
            _pageDirectoryCache.Clear();

            return _messageAggregator.PublishAsync(new PageDirectoryAccessRuleUpdatedMessage()
            {
                PageDirectoryId = accessRule.PageDirectoryId,
                PageDirectoryAccessRuleId = accessRule.PageDirectoryAccessRuleId,
            });
        }

        private async Task<PageDirectoryAccessRule> GetAccessRuleAsync(int id)
        {
            var accessRule = await _dbContext
                .PageDirectoryAccessRules
                .FilterById(id)
                .Include(r => r.Role)
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(accessRule, id);

            return accessRule;
        }

        private async Task<IUserAreaDefinition> GetUserAreaAsync(
            string userAreaCode,
            PageDirectoryAccessRule pageAccessRule,
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

        private async Task<Role> GetRoleAsync(UpdatePageDirectoryAccessRuleCommand command, PageDirectoryAccessRule accessRule)
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
            UpdatePageDirectoryAccessRuleCommand command,
            PageDirectoryAccessRule accessRule,
            IExecutionContext executionContext
            )
        {
            if (command.RoleId == accessRule.RoleId
                && command.UserAreaCode == accessRule.UserAreaCode)
            {
                return;
            }

            var isUnique = await _queryExecutor.ExecuteAsync(new IsPageDirectoryAccessRuleUniqueQuery()
            {
                PageDirectoryId = accessRule.PageDirectoryId,
                PageDirectoryAccessRuleId = accessRule.PageDirectoryAccessRuleId,
                UserAreaCode = command.UserAreaCode,
                RoleId = command.RoleId
            }, executionContext);

            if (!isUnique)
            {
                var propertyName = command.RoleId.HasValue ? nameof(command.RoleId) : nameof(command.UserAreaCode);
                var displayName = command.RoleId.HasValue ? "role" : "user area";
                var message = $"An existing access rule is already configured with this {displayName}.";
                throw new UniqueConstraintViolationException<PageDirectoryAccessRule>(message, propertyName);
            }
        }

        private void UpdateAccessRule(
            PageDirectoryAccessRule accessRule,
            UpdatePageDirectoryAccessRuleCommand command,
            IUserAreaDefinition userArea,
            Role role
            )
        {
            accessRule.UserAreaCode = userArea.UserAreaCode;
            accessRule.Role = role;
            accessRule.RouteAccessRuleViolationActionId = (int)command.ViolationAction;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(UpdatePageDirectoryAccessRuleCommand command)
        {
            yield return new PageDirectoryAccessRuleManagePermission();
        }
    }
}
