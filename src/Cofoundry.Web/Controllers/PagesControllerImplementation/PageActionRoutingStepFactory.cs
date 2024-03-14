namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IPageActionRoutingStepFactory"/>.
/// </summary>
public class PageActionRoutingStepFactory : IPageActionRoutingStepFactory
{
    private readonly IEnumerable<IPageActionRoutingStep> _routingSteps;

    public PageActionRoutingStepFactory(
        ICheckSiteIsSetupRoutingStep checkSiteIsSetupRoutingStep,
        IInitVisualEditorStateRoutingStep initStateRoutingStep,
        ITryFindPageRoutingInfoRoutingStep tryFindPageRoutingInfoRoutingStep,
        IInitUserContextRoutingStep initUserContextRoutingStep,
        IValidateAccessRulesRoutingStep validateAccessRulesRoutingStep,
        IValidateEntityEditModeRoutingStep validateEntityEditModeRoutingStep,
        IValidateEditPermissionsRoutingStep validateEditPermissionsRoutingStep,
        IValidateDraftVersionRoutingStep validateDraftVersionRoutingStep,
        IValidateSpecificVersionRoutingRoutingStep validateSpecificVersionRoutingRoutingStep,
        IGetNotFoundRouteRoutingStep getNotFoundRouteRoutingStep,
        IGetPageRenderDataRoutingStep getPageRenderDataRoutingStep,
        ISetCachePolicyRoutingStep setCachePolicyRoutingStep,
        IGetFinalResultRoutingStep getFinalResultRoutingStep
        )
    {
        // Here we set the default routing steps, which are run in the order they are 
        // declared. Each step can be overridden using the DI system if you need to debug it
        // in an implemenetation.

        var routingSteps = new List<IPageActionRoutingStep>()
        {
            checkSiteIsSetupRoutingStep,
            initStateRoutingStep,
            tryFindPageRoutingInfoRoutingStep,
            initUserContextRoutingStep,
            validateAccessRulesRoutingStep,
            validateEntityEditModeRoutingStep,
            validateEditPermissionsRoutingStep,
            validateDraftVersionRoutingStep,
            validateSpecificVersionRoutingRoutingStep,
            getNotFoundRouteRoutingStep,
            getPageRenderDataRoutingStep,
            setCachePolicyRoutingStep,
            getFinalResultRoutingStep
        };

        _routingSteps = routingSteps;
    }

    /// <inheritdoc/>
    public IEnumerable<IPageActionRoutingStep> Create()
    {
        return _routingSteps;
    }
}
