using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Validate that the currently logged in user can access the route. If
    /// the user fails any access rules checks, then the action associated 
    /// with the rule is carried out e.g. redirect to login, 404, throw error.  
    /// </summary>
    public class ValidateAccessRulesRoutingStep : IValidateAccessRulesRoutingStep
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly ILogger<ValidateAccessRulesRoutingStep> _logger;

        public ValidateAccessRulesRoutingStep(
            IQueryExecutor queryExecutor,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            ILogger<ValidateAccessRulesRoutingStep> logger
            )
        {
            _queryExecutor = queryExecutor;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _logger = logger;
        }

        public Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            // If no page (404) skip this step - it will be handled later
            // Access rules don't apply to Cofoundry admin users, so skip this step
            if (state.PageRoutingInfo == null || state.IsCofoundryAdminUser)
            {
                var skipReason = state.IsCofoundryAdminUser ? "User is Cofoundry admin user" : "no page found";
                _logger.LogInformation("Skipping access rule validation step, {SkipReason}.", skipReason);

                return Task.CompletedTask;
            }

            var accessRuleViolation = state.PageRoutingInfo.CanAccess(state.AmbientUserContext);
            if (accessRuleViolation == null)
            {
                _logger.LogInformation("No access rule violations found.");
            }
            else
            {
                EnforceRuleViolation(controller, state, accessRuleViolation);
            }

            return Task.CompletedTask;
        }

        private void EnforceRuleViolation(Controller controller, PageActionRoutingState state, RouteAccessRule accessRuleViolation)
        {
            _logger.LogInformation("Processing violation action {ViolationAction}.", accessRuleViolation.ViolationAction);

            switch (accessRuleViolation.ViolationAction)
            {
                case RouteAccessRuleViolationAction.NotFound:
                    // Set the route to null and the IGetNotFoundRouteRoutingStep will figure out the correct result
                    state.PageRoutingInfo = null;
                    break;
                case RouteAccessRuleViolationAction.RedirectToLogin:
                    var userArea = _userAreaDefinitionRepository.GetByCode(accessRuleViolation.UserAreaCode);
                    var loginPath = QueryHelpers.AddQueryString(userArea.LoginPath, "ReturnUrl", controller.Request.GetEncodedPathAndQuery());
                    state.Result = new RedirectResult(loginPath, false);
                    break;
                case RouteAccessRuleViolationAction.Error:
                    // Throw an exception, which should be picked up by the global handler and dealt with accordingly.
                    throw new AccessRuleViolationException($"User is not permitted to access {state.InputParameters.Path}.");
                default:
                    throw new NotImplementedException($"{nameof(RouteAccessRuleViolationAction)}.{accessRuleViolation.ViolationAction} not implemented.");
            };
        }
    }
}
