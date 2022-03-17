using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class EntityAccessRuleSetDetailsMapper : IEntityAccessRuleSetDetailsMapper
{
    private readonly ILogger<IEntityAccessRuleSetDetailsMapper> _logger;
    private readonly IQueryExecutor _queryExecutor;

    public EntityAccessRuleSetDetailsMapper(
        ILogger<IEntityAccessRuleSetDetailsMapper> logger,
        IQueryExecutor queryExecutor
        )
    {
        _logger = logger;
        _queryExecutor = queryExecutor;
    }

    public async Task MapAsync<TAccessRule, TEntityAccessRuleSummary>(
        IEntityAccessRestrictable<TAccessRule> dbEntity,
        IEntityAccessRuleSetDetails<TEntityAccessRuleSummary> result,
        IExecutionContext executionContext,
        Action<TAccessRule, TEntityAccessRuleSummary> ruleMapper
        )
        where TAccessRule : IEntityAccessRule
        where TEntityAccessRuleSummary : IEntityAccessRuleSummary, new()
    {
        if (dbEntity == null) throw new ArgumentNullException(nameof(dbEntity));
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));
        if (ruleMapper == null) throw new ArgumentNullException(nameof(ruleMapper));

        result.ViolationAction = ParseViolationAction(dbEntity);

        if (result.AccessRules == null)
        {
            result.AccessRules = new List<TEntityAccessRuleSummary>();
        }

        if (!dbEntity.AccessRules.Any()) return;

        await MapAccessRules(dbEntity, result, executionContext, ruleMapper);
        MapUserAreaForSignInRedirect(dbEntity, result);
    }

    private async Task MapAccessRules<TAccessRule, TEntityAccessRuleSummary>(
        IEntityAccessRestrictable<TAccessRule> dbEntity,
        IEntityAccessRuleSetDetails<TEntityAccessRuleSummary> result,
        IExecutionContext executionContext,
        Action<TAccessRule, TEntityAccessRuleSummary> ruleMapper
        )
        where TAccessRule : IEntityAccessRule
        where TEntityAccessRuleSummary : IEntityAccessRuleSummary, new()
    {
        var userAreas = await _queryExecutor.ExecuteAsync(new GetAllUserAreaMicroSummariesQuery(), executionContext);
        var roleIds = dbEntity
            .AccessRules
            .Where(r => r.RoleId.HasValue)
            .Select(r => r.RoleId.Value)
            .ToArray();

        IDictionary<int, RoleMicroSummary> roles = null;

        if (roleIds.Any())
        {
            roles = await _queryExecutor.ExecuteAsync(new GetRoleMicroSummariesByIdRangeQuery(roleIds), executionContext);
        }

        foreach (var dbRule in dbEntity
            .AccessRules
            .OrderByDefault())
        {
            var rule = new TEntityAccessRuleSummary();
            ruleMapper(dbRule, rule);
            rule.UserArea = userAreas.SingleOrDefault(a => a.UserAreaCode == dbRule.UserAreaCode);
            EntityNotFoundException.ThrowIfNull(rule.UserArea, dbRule.UserAreaCode);

            if (dbRule.RoleId.HasValue)
            {
                rule.Role = roles.GetOrDefault(dbRule.RoleId.Value);
                EntityNotFoundException.ThrowIfNull(rule.Role, dbRule.RoleId.Value);
            }

            result.AccessRules.Add(rule);
        }
    }

    private void MapUserAreaForSignInRedirect<TAccessRule, TEntityAccessRuleSummary>(
        IEntityAccessRestrictable<TAccessRule> dbEntity,
        IEntityAccessRuleSetDetails<TEntityAccessRuleSummary> result
        )
        where TAccessRule : IEntityAccessRule
        where TEntityAccessRuleSummary : IEntityAccessRuleSummary
    {
        if (dbEntity.UserAreaCodeForSignInRedirect != null)
        {
            result.UserAreaForSignInRedirect = result
                .AccessRules
                .Select(r => r.UserArea)
                .Where(a => a.UserAreaCode == dbEntity.UserAreaCodeForSignInRedirect)
                .FirstOrDefault();

            if (result.UserAreaForSignInRedirect == null)
            {
                _logger.LogWarning(
                    "UserAreaCodeForSignInRedirect of value '{UserAreaCodeForSignInRedirect}' is expected to exist in the AccessRules collection, but could not be found.",
                    dbEntity.UserAreaCodeForSignInRedirect
                    );
            }
        }
    }

    private AccessRuleViolationAction ParseViolationAction<TAccessRule>(IEntityAccessRestrictable<TAccessRule> dbEntity) where TAccessRule : IEntityAccessRule
    {
        var violationAction = EnumParser.ParseOrNull<AccessRuleViolationAction>(dbEntity.AccessRuleViolationActionId);

        if (violationAction == null)
        {
            _logger.LogWarning(
                "AccessRuleViolationAction of value {AccessRuleViolationAction} could not be parsed on rule type {TAccessRule}.",
                dbEntity.AccessRuleViolationActionId,
                typeof(TAccessRule).Name
                );
        }

        return violationAction ?? AccessRuleViolationAction.Error;
    }
}
