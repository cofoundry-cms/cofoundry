using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Common mapping functionality for <see cref="EntityAccessRule"/> projections.
/// </summary>
public class EntityAccessRuleSetMapper : IEntityAccessRuleSetMapper
{
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
    private readonly ILogger<EntityAccessRuleSetMapper> _logger;

    public EntityAccessRuleSetMapper(
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        ILogger<EntityAccessRuleSetMapper> logger
        )
    {
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
        _logger = logger;
    }

    public EntityAccessRuleSet Map<TAccessRule>(IEntityAccessRestrictable<TAccessRule> entity)
        where TAccessRule : IEntityAccessRule
    {
        ArgumentNullException.ThrowIfNull(entity);
        MissingIncludeException.ThrowIfNull(entity, e => e.AccessRules);

        if (!entity.AccessRules.Any())
        {
            // no rules, so return null rather than an empty ruleset
            return null;
        }

        var violationAction = EnumParser.ParseOrNull<AccessRuleViolationAction>(entity.AccessRuleViolationActionId);

        if (violationAction == null)
        {
            _logger.LogWarning(
                "AccessRuleViolationAction of value {AccessRuleViolationAction} could not be parsed on rule type {TAccessRule}.",
                entity.AccessRuleViolationActionId,
                typeof(TAccessRule).Name
                );
        }


        var result = new EntityAccessRuleSet()
        {
            ViolationAction = violationAction ?? AccessRuleViolationAction.Error
        };

        if (!string.IsNullOrWhiteSpace(entity.UserAreaCodeForSignInRedirect))
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(entity.UserAreaCodeForSignInRedirect);
            result.UserAreaCodeForSignInRedirect = userArea.UserAreaCode;
        }

        result.AccessRules = entity
            .AccessRules
            .OrderByDefault()
            .Select(r => MapAccessRule(r))
            .ToArray();

        return result;
    }

    private EntityAccessRule MapAccessRule(IEntityAccessRule entityAccessRule)
    {
        var userArea = _userAreaDefinitionRepository.GetRequiredByCode(entityAccessRule.UserAreaCode);

        return new EntityAccessRule()
        {
            UserAreaCode = userArea.UserAreaCode,
            RoleId = entityAccessRule.RoleId
        };
    }
}
