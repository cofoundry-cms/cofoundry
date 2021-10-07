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
    /// Adds a new access rule to a page directory.
    /// </summary>
    public class AddPageDirectoryAccessRuleCommandHandler
        : ICommandHandler<AddPageDirectoryAccessRuleCommand>
        , IPermissionRestrictedCommandHandler<AddPageDirectoryAccessRuleCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly EntityAuditHelper _entityAuditHelper;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IPageDirectoryCache _pageDirectoryCache;
        private readonly IMessageAggregator _messageAggregator;
        private readonly ITransactionScopeManager _transactionScopeFactory;

        public AddPageDirectoryAccessRuleCommandHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            EntityAuditHelper entityAuditHelper,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IPageDirectoryCache pageDirectoryCache,
            IMessageAggregator messageAggregator,
            ITransactionScopeManager transactionScopeFactory
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _entityAuditHelper = entityAuditHelper;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _pageDirectoryCache = pageDirectoryCache;
            _messageAggregator = messageAggregator;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task ExecuteAsync(AddPageDirectoryAccessRuleCommand command, IExecutionContext executionContext)
        {
            var pageDirectory = await GetPageDirectoryAsync(command);
            var userArea = await GetUserAreaAsync(command.UserAreaCode, executionContext);
            var role = await GetRoleAsync(command);

            ValidateRoleIsInUserArea(userArea, role);
            await ValidateIsUniqueAsync(command, executionContext);

            var accessRule = MapAccessRule(command, pageDirectory, userArea, role, executionContext);
            _dbContext.PageDirectoryAccessRules.Add(accessRule);

            await _dbContext.SaveChangesAsync();
            await _transactionScopeFactory.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(accessRule));

            command.OutputPageDirectoryAccessRuleId = accessRule.PageDirectoryAccessRuleId;
        }

        private async Task<PageDirectory> GetPageDirectoryAsync(AddPageDirectoryAccessRuleCommand command)
        {
            var pageDirectory = await _dbContext
                .PageDirectories
                .FilterById(command.PageDirectoryId)
                .SingleOrDefaultAsync();

            if (pageDirectory == null)
            {
                throw ValidationErrorException.CreateWithProperties("Directory does not exist.", nameof(command.PageDirectoryId));
            }

            return pageDirectory;
        }

        private async Task<IUserAreaDefinition> GetUserAreaAsync(string userAreaCode, IExecutionContext executionContext)
        {
            var userArea = _userAreaDefinitionRepository.GetByCode(userAreaCode);
            await _commandExecutor.ExecuteAsync(new EnsureUserAreaExistsCommand(userArea.UserAreaCode), executionContext);

            return userArea;
        }

        private async Task<Role> GetRoleAsync(AddPageDirectoryAccessRuleCommand command)
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

        private async Task ValidateIsUniqueAsync(AddPageDirectoryAccessRuleCommand command, IExecutionContext executionContext)
        {
            var isUnique = await _queryExecutor.ExecuteAsync(new IsPageDirectoryAccessRuleUniqueQuery()
            {
                PageDirectoryId = command.PageDirectoryId,
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


        private PageDirectoryAccessRule MapAccessRule(
            AddPageDirectoryAccessRuleCommand command,
            PageDirectory pageDirectory,
            IUserAreaDefinition userArea,
            Role role,
            IExecutionContext executionContext
            )
        {

            var accessRule = new PageDirectoryAccessRule()
            {
                PageDirectory = pageDirectory,
                Role = role,
                UserAreaCode = userArea.UserAreaCode,
                RouteAccessRuleViolationActionId = (int)command.ViolationAction
            };

            _entityAuditHelper.SetCreated(accessRule, executionContext);

            return accessRule;
        }

        private Task OnTransactionComplete(PageDirectoryAccessRule accessRule)
        {
            _pageDirectoryCache.Clear();

            return _messageAggregator.PublishAsync(new PageDirectoryAccessRuleAddedMessage()
            {
                PageDirectoryId = accessRule.PageDirectoryId,
                PageDirectoryAccessRuleId = accessRule.PageDirectoryAccessRuleId
            });
        }

        public IEnumerable<IPermissionApplication> GetPermissions(AddPageDirectoryAccessRuleCommand command)
        {
            yield return new PageDirectoryAccessRuleManagePermission();
        }
    }
}
