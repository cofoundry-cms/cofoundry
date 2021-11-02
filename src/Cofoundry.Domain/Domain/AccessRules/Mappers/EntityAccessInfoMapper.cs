using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class EntityAccessInfoMapper : IEntityAccessInfoMapper
    {
        private readonly ILogger<IEntityAccessInfoMapper> _logger;
        private readonly IQueryExecutor _queryExecutor;

        public EntityAccessInfoMapper(
            ILogger<IEntityAccessInfoMapper> logger,
            IQueryExecutor queryExecutor
            )
        {
            _logger = logger;
            _queryExecutor = queryExecutor;
        }

        public async Task MapAsync<TAccessRule, TEntityAccessRuleSummary>(
            IEntityAccessRestrictable<TAccessRule> dbEntity,
            IEntityAccessInfo<TEntityAccessRuleSummary> result,
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

            var violationAction = EnumParser.ParseOrNull<AccessRuleViolationAction>(dbEntity.AccessRuleViolationActionId);

            if (violationAction == null)
            {
                _logger.LogWarning(
                    "AccessRuleViolationAction of value {AccessRuleViolationAction} could not be parsed on rule type {TAccessRule}.",
                    dbEntity.AccessRuleViolationActionId,
                    typeof(TAccessRule).Name
                    );
            }

            result.UserAreaCodeForLoginRedirect = dbEntity.UserAreaCodeForLoginRedirect;
            result.ViolationAction = violationAction.Value;

            if (result.AccessRules == null)
            {
                result.AccessRules = new List<TEntityAccessRuleSummary>();
            }

            if (!dbEntity.AccessRules.Any()) return;

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
    }
}
