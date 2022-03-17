using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class UpdateAccessRuleSetCommandHelper : IUpdateAccessRuleSetCommandHelper
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IDomainRepository _domainRepository;
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
    private readonly EntityAuditHelper _entityAuditHelper;

    public UpdateAccessRuleSetCommandHelper(
        CofoundryDbContext dbContext,
        IDomainRepository domainRepository,
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        EntityAuditHelper entityAuditHelper
        )
    {
        _dbContext = dbContext;
        _domainRepository = domainRepository;
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
        _entityAuditHelper = entityAuditHelper;
    }

    public async Task UpdateAsync<TAccessRule, TAddOrUpdateAccessRuleCommand>(
        IEntityAccessRestrictable<TAccessRule> entity,
        UpdateAccessRuleSetCommandBase<TAddOrUpdateAccessRuleCommand> command,
        IExecutionContext executionContext
        )
        where TAccessRule : IEntityAccessRule, new()
        where TAddOrUpdateAccessRuleCommand : AddOrUpdateAccessRuleCommandBase
    {
        var accessRules = EnumerableHelper.Enumerate(command.AccessRules);
        var roles = await GetRolesAsync(accessRules);
        var userAreas = await GetUserAreasAsync(accessRules, executionContext);

        DeleteRules(entity, accessRules);
        UpdateRules(entity, accessRules, roles, userAreas);
        AddRules(entity, accessRules, roles, userAreas, executionContext);
        UpdateEntity(entity, command, userAreas);
    }

    private async Task<Dictionary<int, Role>> GetRolesAsync(
        IEnumerable<AddOrUpdateAccessRuleCommandBase> accessRules
        )
    {
        var roleIds = accessRules
            .Where(r => r.RoleId.HasValue)
            .Select(r => (int)r.RoleId)
            .Distinct()
            .ToArray();

        var roles = await _dbContext
            .Roles
            .Where(r => roleIds.Contains(r.RoleId))
            .ToDictionaryAsync(r => r.RoleId);

        return roles;
    }

    private async Task<Dictionary<string, IUserAreaDefinition>> GetUserAreasAsync(
        IEnumerable<AddOrUpdateAccessRuleCommandBase> accessRules,
        IExecutionContext executionContext
        )
    {
        var userAreas = accessRules
            .Select(r => r.UserAreaCode)
            .Distinct()
            .Select(_userAreaDefinitionRepository.GetRequiredByCode)
            .ToDictionary(k => k.UserAreaCode);

        foreach (var userArea in userAreas)
        {
            await _domainRepository
                .WithContext(executionContext)
                .ExecuteCommandAsync(new EnsureUserAreaExistsCommand(userArea.Key));
        }

        return userAreas;
    }

    private static void DeleteRules<TAccessRule>(IEntityAccessRestrictable<TAccessRule> entity, IEnumerable<AddOrUpdateAccessRuleCommandBase> accessRules)
        where TAccessRule : IEntityAccessRule
    {
        var rulesToDelete = entity
            .AccessRules
            .Where(r => !accessRules.Any(x => x.GetId() == r.GetId()))
            .ToList();

        foreach (var rule in rulesToDelete)
        {
            entity.AccessRules.Remove(rule);
        }
    }

    private void UpdateRules<TAccessRule>(
        IEntityAccessRestrictable<TAccessRule> entity,
        IEnumerable<AddOrUpdateAccessRuleCommandBase> accessRules,
        Dictionary<int, Role> roles,
        Dictionary<string, IUserAreaDefinition> userAreas)
        where TAccessRule : IEntityAccessRule
    {
        foreach (var updateRuleCommand in accessRules.Where(r => r.GetId().HasValue))
        {
            var id = updateRuleCommand.GetId();
            var dbRule = entity
                .AccessRules
                .FilterById(id.Value)
                .SingleOrDefault();

            EntityNotFoundException.ThrowIfNull(dbRule, id);

            var userAreaDefinition = userAreas.GetOrDefault(updateRuleCommand.UserAreaCode);
            EntityNotFoundException.ThrowIfNull(userAreaDefinition, updateRuleCommand.RoleId);
            dbRule.UserAreaCode = userAreaDefinition.UserAreaCode;

            if (updateRuleCommand.RoleId.HasValue)
            {
                var role = roles.GetOrDefault(updateRuleCommand.RoleId.Value);
                EntityNotFoundException.ThrowIfNull(role, updateRuleCommand.RoleId);
                ValidateRoleIsInUserArea(userAreaDefinition, role);
                dbRule.Role = role;
            }
            else
            {
                dbRule.Role = null;
            }
        }
    }

    private void AddRules<TAccessRule>(
        IEntityAccessRestrictable<TAccessRule> entity,
        IEnumerable<AddOrUpdateAccessRuleCommandBase> accessRules,
        Dictionary<int, Role> roles,
        Dictionary<string, IUserAreaDefinition> userAreas,
        IExecutionContext executionContext
        )
        where TAccessRule : IEntityAccessRule, new()
    {
        foreach (var addRuleCommand in accessRules.Where(r => !r.GetId().HasValue))
        {
            var dbRule = new TAccessRule();

            var userAreaDefinition = userAreas.GetOrDefault(addRuleCommand.UserAreaCode);
            EntityNotFoundException.ThrowIfNull(userAreaDefinition, addRuleCommand.RoleId);
            dbRule.UserAreaCode = userAreaDefinition.UserAreaCode;

            if (addRuleCommand.RoleId.HasValue)
            {
                var role = roles.GetOrDefault(addRuleCommand.RoleId.Value);
                EntityNotFoundException.ThrowIfNull(role, addRuleCommand.RoleId);
                ValidateRoleIsInUserArea(userAreaDefinition, role);
                dbRule.Role = role;
            }

            _entityAuditHelper.SetCreated(dbRule, executionContext);

            entity.AccessRules.Add(dbRule);
        }
    }

    private static void UpdateEntity<TAccessRule, TAddOrUpdateAccessRuleCommand>(
        IEntityAccessRestrictable<TAccessRule> entity,
        UpdateAccessRuleSetCommandBase<TAddOrUpdateAccessRuleCommand> command,
        Dictionary<string, IUserAreaDefinition> userAreas
        )
        where TAccessRule : IEntityAccessRule
        where TAddOrUpdateAccessRuleCommand : AddOrUpdateAccessRuleCommandBase
    {
        entity.AccessRuleViolationActionId = (int)command.ViolationAction;

        if (string.IsNullOrEmpty(command.UserAreaCodeForSignInRedirect))
        {
            entity.UserAreaCodeForSignInRedirect = null;
        }
        else
        {
            var userArea = userAreas.GetOrDefault(command.UserAreaCodeForSignInRedirect);
            EntityNotFoundException.ThrowIfNull(userArea, command.UserAreaCodeForSignInRedirect);
            entity.UserAreaCodeForSignInRedirect = userArea.UserAreaCode;
        }
    }

    private void ValidateRoleIsInUserArea(IUserAreaDefinition userAreaDefinition, Role role)
    {
        if (role != null && role.UserAreaCode != userAreaDefinition.UserAreaCode)
        {
            throw ValidationErrorException.CreateWithProperties($"This role is not in the {userAreaDefinition.Name} user area.", nameof(role.RoleId));
        }
    }
}
