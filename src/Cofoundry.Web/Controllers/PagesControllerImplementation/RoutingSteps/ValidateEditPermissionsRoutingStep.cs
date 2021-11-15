using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Checks to see if there is a valid page version for the request, if not
    /// one is created.
    /// </summary>
    public class ValidateEditPermissionsRoutingStep : IValidateEditPermissionsRoutingStep
    {
        private readonly IPermissionValidationService _permissionValidationService;

        public ValidateEditPermissionsRoutingStep(
            IPermissionValidationService permissionValidationService
            )
        {
            _permissionValidationService = permissionValidationService;
        }

        public Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            var pageRoutingInfo = state.PageRoutingInfo;
            if (pageRoutingInfo == null || state.VisualEditorState.VisualEditorMode != VisualEditorMode.Edit) return Task.CompletedTask;

            if (pageRoutingInfo.CustomEntityRoute != null
                && state.InputParameters.IsEditingCustomEntity
                )
            {
                _permissionValidationService.EnforceCustomEntityPermission<CustomEntityUpdatePermission>(pageRoutingInfo.CustomEntityRoute.CustomEntityDefinitionCode, state.CofoundryAdminUserContext);
            }

            if (!state.InputParameters.IsEditingCustomEntity)
            {
                _permissionValidationService.EnforcePermission<PageUpdatePermission>(state.CofoundryAdminUserContext);
            }

            return Task.CompletedTask;
        }
    }
}
