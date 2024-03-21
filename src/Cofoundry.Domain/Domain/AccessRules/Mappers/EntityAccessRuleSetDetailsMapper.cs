using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IEntityAccessRuleSetDetailsMapper"/>.
/// </summary>
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

    /// <inheritdoc/>
    public async Task MapAsync<TAccessRule, TEntityAccessRuleSummary>(
        IEntityAccessRestrictable<TAccessRule> dbEntity,
        IEntityAccessRuleSetDetails<TEntityAccessRuleSummary> result,
        IExecutionContext executionContext,
        Action<TAccessRule, TEntityAccessRuleSummary> ruleMapper
        )
        where TAccessRule : IEntityAccessRule
        where TEntityAccessRuleSummary : IEntityAccessRuleSummary, new()
    {
        ArgumentNullException.ThrowIfNull(dbEntity);
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(executionContext);
        ArgumentNullException.ThrowIfNull(ruleMapper);

        result.ViolationAction = ParseViolationAction(dbEntity);

        if (dbEntity.AccessRules.Count == 0)
        {
            return;
        }

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
            .Select(r => r.RoleId)
            .WhereNotNull()
            .ToArray();

        IDictionary<int, RoleMicroSummary>? roles = null;

        if (roleIds.Length != 0)
        {
            roles = await _queryExecutor.ExecuteAsync(new GetRoleMicroSummariesByIdRangeQuery(roleIds), executionContext);
        }

        var accessRules = result.AccessRules.ToList();

        foreach (var dbRule in dbEntity
            .AccessRules
            .OrderByDefault())
        {
            var rule = new TEntityAccessRuleSummary();
            ruleMapper(dbRule, rule);

            var userArea = userAreas.SingleOrDefault(a => a.UserAreaCode == dbRule.UserAreaCode);
            EntityNotFoundException.ThrowIfNull(userArea, dbRule.UserAreaCode);
            rule.UserArea = userArea;

            if (dbRule.RoleId.HasValue)
            {
                rule.Role = roles?.GetOrDefault(dbRule.RoleId.Value);
                EntityNotFoundException.ThrowIfNull(rule.Role, dbRule.RoleId.Value);
            }

            accessRules.Add(rule);
        }

        result.AccessRules = accessRules;
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
