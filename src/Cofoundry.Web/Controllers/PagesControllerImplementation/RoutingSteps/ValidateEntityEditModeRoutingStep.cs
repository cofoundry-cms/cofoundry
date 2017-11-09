using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Validates the PageActionRoutingState.IsEditingCustomEntity property. We set 
    /// PageActionRoutingState.IsEditingCustomEntity to true by default because we want to prefer
    /// entity editing when it is available over page editing, so here we check to see
    /// if setting IsEditingCustomEntity to true is invalid and revert it.
    /// </summary>
    public class ValidateEntityEditModeRoutingStep : IValidateEntityEditModeRoutingStep
    {
        private readonly IPermissionValidationService _permissionValidationService;

        public ValidateEntityEditModeRoutingStep(
            IPermissionValidationService permissionValidationService
            )
        {
            _permissionValidationService = permissionValidationService;
        }

        public Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            var pageRoutingInfo = state.PageRoutingInfo;
            if (pageRoutingInfo == null) return Task.CompletedTask;

            if (state.InputParameters.IsEditingCustomEntity &&
                (pageRoutingInfo.CustomEntityRoute == null 
                || !state.IsCofoundryAdminUser
                || !_permissionValidationService.HasCustomEntityPermission<CustomEntityUpdatePermission>(pageRoutingInfo.CustomEntityRoute.CustomEntityDefinitionCode, state.CofoundryAdminUserContext))
                )
            {
                state.InputParameters.IsEditingCustomEntity = false;
            }

            return Task.CompletedTask;
        }
    }
}
