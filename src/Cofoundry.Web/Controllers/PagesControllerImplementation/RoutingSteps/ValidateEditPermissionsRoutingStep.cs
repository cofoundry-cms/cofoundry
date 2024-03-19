﻿using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

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
        EntityInvalidOperationException.ThrowIfNull(state, state.VisualEditorState);

        var pageRoutingInfo = state.PageRoutingInfo;
        if (pageRoutingInfo == null || state.VisualEditorState.VisualEditorMode != VisualEditorMode.Edit)
        {
            return Task.CompletedTask;
        }

        EntityInvalidOperationException.ThrowIfNull(state, state.CofoundryAdminUserContext);

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
