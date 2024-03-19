﻿using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

/// <inheritdoc/>
public class InitUserContextRoutingStep : IInitUserContextRoutingStep
{
    private readonly IUserContextService _userContextService;
    private readonly IExecutionContextFactory _executionContextFactory;

    public InitUserContextRoutingStep(
        IUserContextService userContextService,
        IExecutionContextFactory executionContextFactory
        )
    {
        _userContextService = userContextService;
        _executionContextFactory = executionContextFactory;
    }

    public async Task ExecuteAsync(Controller controller, PageActionRoutingState state)
    {
        // The ambient auth scheme might not be the cofoundry admin scheme
        // So we will attempt to find the cofoundry user to execute the contoller with
        // falling back to the user authenticated with the ambient scheme
        state.AmbientUserContext = await _userContextService.GetCurrentContextAsync();

        IUserContext cofoundryUserContext;
        if (state.AmbientUserContext.IsCofoundryUser())
        {
            cofoundryUserContext = state.AmbientUserContext;
        }
        else
        {
            cofoundryUserContext = await _userContextService.GetCurrentContextByUserAreaAsync(CofoundryAdminUserArea.Code);
        }

        if (cofoundryUserContext.IsCofoundryUser())
        {
            state.IsCofoundryAdminUser = true;
            state.CofoundryAdminUserContext = cofoundryUserContext;
            state.CofoundryAdminExecutionContext = _executionContextFactory.Create(state.CofoundryAdminUserContext);
        }
    }
}
