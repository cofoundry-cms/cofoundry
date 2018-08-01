using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// If the request is for a specific page version, we validate that the user has permission to see 
    /// that version and that the version requested is valid. If it is not valid then the version
    /// parameters are discarded.
    /// </summary>
    public class ValidateSpecificVersionRoutingRoutingStep : IValidateSpecificVersionRoutingRoutingStep
    {
        private readonly INotFoundViewHelper _notFoundViewHelper;
        private readonly IPageRouteLibrary _pageRouteLibrary;

        public ValidateSpecificVersionRoutingRoutingStep(
            INotFoundViewHelper notFoundViewHelper, 
            IPageRouteLibrary pageRouteLibrary
            )
        {
            _notFoundViewHelper = notFoundViewHelper;
            _pageRouteLibrary = pageRouteLibrary;
        }

        public async Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            // Ensure that non-authenticated users can't access previous versions
            if (state.VisualEditorState.VisualEditorMode != VisualEditorMode.SpecificVersion)
            {
                state.InputParameters.VersionId = null;
            }
            else if (state.PageRoutingInfo != null)
            {
                var versionRoute = state.PageRoutingInfo.GetVersionRoute(
                    state.InputParameters.IsEditingCustomEntity,
                    state.VisualEditorState.GetPublishStatusQuery(),
                    state.InputParameters.VersionId);

                // If this isn't an old version of a page, set the VisualEditorMode accordingly.
                if (versionRoute != null)
                {
                    if (versionRoute.WorkFlowStatus == WorkFlowStatus.Draft)
                    {
                        var url = _pageRouteLibrary.VisualEditor(
                            state.PageRoutingInfo, 
                            VisualEditorMode.Preview, 
                            state.InputParameters.IsEditingCustomEntity
                            );

                        state.Result = controller.Redirect(url);
                    }
                    else if (versionRoute.IsLatestPublishedVersion && state.PageRoutingInfo.IsPublished())
                    {
                        var url = _pageRouteLibrary.VisualEditor(
                            state.PageRoutingInfo,
                            VisualEditorMode.Live,
                            state.InputParameters.IsEditingCustomEntity
                            );

                        state.Result = controller.Redirect(url);
                    }
                }
                else
                {
                    // Could not find a version, id must be invalid
                    state.Result = await _notFoundViewHelper.GetViewAsync(controller);
                }
            }
        }
    }
}
