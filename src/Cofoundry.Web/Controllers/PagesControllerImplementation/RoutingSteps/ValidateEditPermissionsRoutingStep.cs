using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Checks to see if there is a valid page version for the request, if not
    /// one is created.
    /// </summary>
    public class ValidateEditPermissionsRoutingStep : IValidateEditPermissionsRoutingStep
    {
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly ICommandExecutor _commandExecutor;

        public ValidateEditPermissionsRoutingStep(
            IPermissionValidationService permissionValidationService,
            ICommandExecutor commandExecutor
            )
        {
            _permissionValidationService = permissionValidationService;
            _commandExecutor = commandExecutor;
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

        private GetPageRoutingInfoByPathQuery GetPageRoutingInfoQuery(PageActionRoutingState state)
        {
            var pageQuery = new GetPageRoutingInfoByPathQuery()
            {
                Path = state.InputParameters.Path,
                IncludeUnpublished = state.VisualEditorState.VisualEditorMode != VisualEditorMode.Live
            };

            if (state.Locale != null)
            {
                pageQuery.LocaleId = state.Locale.LocaleId;
            }

            return pageQuery;
        }
    }
}
