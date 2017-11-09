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
        public Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            // Ensure that non-authenticated users can't access previous versions
            if (state.VisualEditorMode != VisualEditorMode.SpecificVersion)
            {
                state.InputParameters.VersionId = null;
            }
            else if (state.PageRoutingInfo != null)
            {
                var versionRoute = state.PageRoutingInfo.GetVersionRoute(
                    state.InputParameters.IsEditingCustomEntity,
                    state.VisualEditorMode.ToPublishStatusQuery(),
                    state.InputParameters.VersionId);

                // If this isn't an old version of a page, set the VisualEditorMode accordingly.
                if (versionRoute != null)
                {
                    switch (versionRoute.WorkFlowStatus)
                    {
                        case Cofoundry.Domain.WorkFlowStatus.Draft:
                            state.VisualEditorMode = VisualEditorMode.Draft;
                            break;
                        case Cofoundry.Domain.WorkFlowStatus.Published:
                            state.VisualEditorMode = VisualEditorMode.Live;
                            break;
                    }
                }
                else
                {
                    // Could not find a version, id must be invalid
                    state.InputParameters.VersionId = null;
                }
            }

            return Task.CompletedTask;
        }
    }
}
