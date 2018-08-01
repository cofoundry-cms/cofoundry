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
        private readonly IVisualEditorStateService _visualEditorStateService;

        public InitStateRoutingStep(
            IUserContextService userContextService,
            ContentSettings contentSettings,
            IExecutionContextFactory executionContextFactory,
            IVisualEditorStateService visualEditorStateService
            )
        {
            _userContextService = userContextService;
            _contentSettings = contentSettings;
            _executionContextFactory = executionContextFactory;
            _visualEditorStateService = visualEditorStateService;
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

            state.VisualEditorState = await _visualEditorStateService.GetCurrentAsync();
        }
    }
}
