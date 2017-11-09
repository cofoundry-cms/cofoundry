using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web
{
    /// <summary>
    /// Initialises the key parameters of the PageActionRoutingState
    /// object e.g. the UserContext and VisualEditorMode properties
    /// </summary>
    public class InitStateRoutingStep : IInitStateRoutingStep
    {
        private readonly IUserContextService _userContextService;
        private readonly ContentSettings _contentSettings;
        private readonly IExecutionContextFactory _executionContextFactory;

        public InitStateRoutingStep(
            IUserContextService userContextService,
            ContentSettings contentSettings,
            IExecutionContextFactory executionContextFactory
            )
        {
            _userContextService = userContextService;
            _contentSettings = contentSettings;
            _executionContextFactory = executionContextFactory;
        }

        public async Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            // The ambient auth schema might not be the cofoundry admin scheme
            // So we will attempt to find the cofoundry user to execute the contoller with
            // falling back to the user authenticated with the ambient scheme
            state.AmbientUserContext = await _userContextService.GetCurrentContextAsync();
            IUserContext cofoundryUserContext = null;

            if (state.AmbientUserContext.IsCofoundryUser())
            {
                cofoundryUserContext = state.AmbientUserContext;
            }
            else
            {
                cofoundryUserContext = await _userContextService.GetCurrentContextByUserAreaAsync(CofoundryAdminUserArea.AreaCode);
            }

            if (cofoundryUserContext.IsCofoundryUser())
            {
                state.IsCofoundryAdminUser = true;
                state.CofoundryAdminUserContext = cofoundryUserContext;
                state.CofoundryAdminExecutionContext = _executionContextFactory.Create(state.CofoundryAdminUserContext);
            }

            // Work out whether to view the page in live/draft/edit mode.
            // We use live by default (for logged out users) or for authenticated
            // users we can show draft too.
            var visualEditorMode = VisualEditorMode.Live;
            if (state.IsCofoundryAdminUser)
            {
                if (state.InputParameters.VersionId.HasValue)
                {
                    visualEditorMode = VisualEditorMode.SpecificVersion;
                }
                else if (!Enum.TryParse(state.InputParameters.VisualEditorMode, true, out visualEditorMode))
                {
                    visualEditorMode = VisualEditorMode.Any;
                }
            }
            else if (_contentSettings.AlwaysShowUnpublishedData)
            {
                // We can optionally set the visual editor mode to any - ie show draft and published pages
                // This is used in scenarios where devs are making modifications against a live db using a 
                // local debug version of the site but aren't ready to publish the pages yet.
                visualEditorMode = VisualEditorMode.Any;
            }
            state.VisualEditorMode = visualEditorMode;
        }
    }
}
