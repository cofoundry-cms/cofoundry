using Cofoundry.Core;
using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;
using System;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Common mapping functionality for <see cref="RouteAccessRule"/> projections.
    /// </summary>
    public class RouteAccessRuleMapper : IRouteAccessRuleMapper
    {
        private readonly ILogger<RouteAccessRuleMapper> _logger;

        public RouteAccessRuleMapper(
            ILogger<RouteAccessRuleMapper> logger
            )
        {
            _logger = logger;
        }

        /// <summary>
        /// Maps an entity-specific <see cref="IEntityAccessRule"/> data to an <see cref="RouteAccessRule"/>.
        /// </summary>
        /// <param name="entityAccessRule">Database object to map. Cannot be <see langword="null"/>.</param>
        public RouteAccessRule Map<TAccessRule>(TAccessRule entityAccessRule)
            where TAccessRule : IEntityAccessRule
        {
            if (entityAccessRule == null) throw new ArgumentNullException(nameof(entityAccessRule));

            var violationAction = EnumParser.ParseOrNull<RouteAccessRuleViolationAction>(entityAccessRule.RouteAccessRuleViolationActionId);

            if (violationAction == null)
            {
                _logger.LogWarning(
                    "RouteAccessRuleViolationActionId of value {RouteAccessRuleViolationActionId} could not be parsed on rule type {TAccessRule}.",
                    entityAccessRule.RouteAccessRuleViolationActionId,
                    typeof(TAccessRule).Name
                    );
            }

            return new RouteAccessRule()
            {
                UserAreaCode = entityAccessRule.UserAreaCode,
                RoleId = entityAccessRule.RoleId,
                ViolationAction = violationAction ?? RouteAccessRuleViolationAction.Error
            };
        }
    }
}
