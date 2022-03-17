using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

/// <inheritdoc/>
public class InitVisualEditorStateRoutingStep : IInitVisualEditorStateRoutingStep
{
    private readonly IVisualEditorStateService _visualEditorStateService;

    public InitVisualEditorStateRoutingStep(
        IVisualEditorStateService visualEditorStateService
        )
    {
        _visualEditorStateService = visualEditorStateService;
    }

    public async Task ExecuteAsync(Controller controller, PageActionRoutingState state)
    {
        state.VisualEditorState = await _visualEditorStateService.GetCurrentAsync();
    }
}
