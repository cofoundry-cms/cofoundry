using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <inheritdoc/>
    public class ValidateAccessRulesRoutingStep : IValidateAccessRulesRoutingStep
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IUserContextService _userContextService;
        private readonly IUserSessionService _userSessionService;
        private readonly ILogger<ValidateAccessRulesRoutingStep> _logger;

        public ValidateAccessRulesRoutingStep(
            IQueryExecutor queryExecutor,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IUserContextService userContextService,
            IUserSessionService userSessionService,
            ILogger<ValidateAccessRulesRoutingStep> logger
            )
        {
            _queryExecutor = queryExecutor;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _userContextService = userContextService;
            _userSessionService = userSessionService;
            _logger = logger;
        }

        public async Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (state == null) throw new ArgumentNullException(nameof(state));
            EntityInvalidOperationException.ThrowIfNull(state, r => r.AmbientUserContext);

            // If no page (404) skip this step - it will be handled later
            // Access rules don't apply to Cofoundry admin users, so skip this step
            var isAmbientContextCofoundryAdmin = state.AmbientUserContext.IsCofoundryUser();
            if (state.PageRoutingInfo == null || isAmbientContextCofoundryAdmin)
            {
                var skipReason = isAmbientContextCofoundryAdmin ? "User is Cofoundry admin user" : "no page found";
                _logger.LogDebug("Skipping access rule validation step, {SkipReason}.", skipReason);

                return;
            }

            var accessRuleViolation = await FindAccessRuleViolation(state);

            if (accessRuleViolation == null)
            {
                _logger.LogDebug("No access rule violations found.");
            }
            else
            {
                EnforceRuleViolation(controller, state, accessRuleViolation);
            }
        }

        private async Task<EntityAccessRuleSet> FindAccessRuleViolation(PageActionRoutingState state)
        {
            var accessRuleViolation = state.PageRoutingInfo.ValidateAccess(state.AmbientUserContext);

            // If the user associated with the ambient context is not authorized, then we need to check
            // to see if the user is logged into any other user areas that are permitted to access the route.
            if (accessRuleViolation != null)
            {
                // For user areas, only the topmost ruleset matters
                var relavantRuleSet = EnumerateAccessRuleSets(state.PageRoutingInfo).FirstOrDefault();

                // Find any other user areas to check
                // more than one custom user area should be a rare occurence
                // more than two is rare indeed, but we should account for it and use determanistic ordering
                var userAreaCodesToCheck = relavantRuleSet
                    .AccessRules
                    .Select(r => r.UserAreaCode)
                    .Where(r => r != state.AmbientUserContext.UserArea?.UserAreaCode)
                    .Distinct()
                    .OrderBy(r => r);

                foreach (var userAreaCode in userAreaCodesToCheck)
                {
                    var context = await _userContextService.GetCurrentContextByUserAreaAsync(userAreaCode);
                    var ruleViolation = state.PageRoutingInfo.ValidateAccess(context);
                    if (ruleViolation == null)
                    {
                        _logger.LogDebug(
                            "User is logged into non-default user area {UserAreaCode} that passes access rule validation, switching ambient context to userId {UserId}.",
                            context.UserArea.UserAreaCode,
                            context.UserId
                            );

                        // the user is logged into an alternative user area that matches one of the rules
                        // so we should set it as the ambient user context. This mimics the behaviour of 
                        // AuthorizeAttribute in ASP.NET where the auth attribute determines the ambient
                        // auth scheme.
                        await _userSessionService.SetAmbientUserAreaAsync(context.UserArea.UserAreaCode);
                        state.AmbientUserContext = context;
                        accessRuleViolation = null;
                        break;
                    }
                }
            }

            return accessRuleViolation;
        }

        public IEnumerable<EntityAccessRuleSet> EnumerateAccessRuleSets(PageRoutingInfo pageRoutingInfo)
        {
            EntityInvalidOperationException.ThrowIfNull(pageRoutingInfo, r => r.PageRoute);
            EntityInvalidOperationException.ThrowIfNull(pageRoutingInfo.PageRoute, r => r.PageDirectory);

            if (pageRoutingInfo.PageRoute.AccessRuleSet != null)
            {
                yield return pageRoutingInfo.PageRoute.AccessRuleSet;
            }

            foreach (var ruleSet in EnumerableHelper.Enumerate(pageRoutingInfo.PageRoute.PageDirectory.AccessRuleSets))
            {
                yield return ruleSet;
            }
        }

        private void EnforceRuleViolation(Controller controller, PageActionRoutingState state, EntityAccessRuleSet accessRuleViolation)
        {
            if (!state.AmbientUserContext.IsSignedIn() && accessRuleViolation.ShouldTryRedirect())
            {
                _logger.LogInformation("User not authenticated, redirecting to sign in page for user area {UserAreaCodeForLoginRedirect}.", accessRuleViolation.UserAreaCodeForSignInRedirect);
                var challengeScheme = AuthenticationSchemeNames.UserArea(accessRuleViolation.UserAreaCodeForSignInRedirect);
                state.Result = new ChallengeResult(challengeScheme);
                return;
            }

            _logger.LogInformation("Processing violation action {ViolationAction}.", accessRuleViolation.ViolationAction);
            switch (accessRuleViolation.ViolationAction)
            {
                case AccessRuleViolationAction.NotFound:
                    // Set the route to null and the IGetNotFoundRouteRoutingStep will figure out the correct result
                    state.PageRoutingInfo = null;
                    break;
                case AccessRuleViolationAction.Error:
                    // Throw an exception, which should be picked up by the global handler and dealt with accordingly.
                    throw new AccessRuleViolationException($"User is not permitted to access {state.InputParameters.Path}.");
                default:
                    throw new NotImplementedException($"{nameof(AccessRuleViolationAction)}.{accessRuleViolation.ViolationAction} not implemented.");
            };
        }
    }
}
