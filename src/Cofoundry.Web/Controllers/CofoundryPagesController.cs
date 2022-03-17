using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Web;

/// <summary>
/// Main controller for handling Cofoundry page routing, redirection and not found errors. This route
/// is configured last so all other controller routes are scanned first before falling back to this. 
/// </summary>
public class CofoundryPagesController : Controller
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly IPageActionRoutingStepFactory _pageActionRoutingStepFactory;
    private readonly ILogger<CofoundryPagesController> _logger;

    public CofoundryPagesController(
        IQueryExecutor queryExecutor,
        IPageActionRoutingStepFactory pageActionRoutingStepFactory,
        ILogger<CofoundryPagesController> logger
        )
    {
        _queryExecutor = queryExecutor;
        _pageActionRoutingStepFactory = pageActionRoutingStepFactory;
        _logger = logger;
    }

    /// <summary>
    /// Action for all dynamic page routes.
    /// </summary>
    /// <param name="path">The raw, relative path of the page without querystring.</param>
    /// <param name="mode">
    /// This string maps to the enum <see cref="VisualEditorMode"/> and is used in the
    /// visual editor in the admin site. The value is not parsed here as the admin panel
    /// may not be installed, instead it should be parsed in the admin panel
    /// <see cref="IVisualEditorStateService"/> implementation.
    /// </param>
    /// <param name="version">
    /// Optionally a VersionId can be specified to
    /// view a specific version of a page or custom entity.
    /// </param>
    /// <param name="editType">
    /// When routing to a custom entity this determines if we are editing the 
    /// custom entity or the overall page template. Both cannot be edited at the same
    /// time since it would be confusing to manage both version states.
    /// </param>
    public async Task<IActionResult> Page(
        string path,
        string mode,
        int? version = null,
        string editType = "entity"
        )
    {
        _logger.LogInformation("Processing route {Path} with visual editor mode '{VisualEditorMode}', page version Id '{PageVersionId}' and edit-type '{EditType}'", path, mode, version, editType);

        // Init state
        var state = new PageActionRoutingState();
        state.Locale = await GetLocaleAsync();

        state.InputParameters = new PageActionInputParameters()
        {
            Path = path,
            VersionId = version,
            IsEditingCustomEntity = editType == "entity"
        };


        // Run through the pipline in order
        foreach (var method in _pageActionRoutingStepFactory.Create())
        {
            _logger.LogDebug("Executing step {StepType}", method.GetType());
            await method.ExecuteAsync(this, state);
            // If we get an action result, do an early return
            if (state.Result != null)
            {
                _logger.LogDebug("Step {StepType} returned a result.", method.GetType());
                return state.Result;
            }
        }

        // We should never get here!
        throw new InvalidOperationException("Unknown Page Routing State");
    }

    private async Task<ActiveLocale> GetLocaleAsync()
    {
        var activeLocale = await _queryExecutor.ExecuteAsync(new GetCurrentActiveLocaleQuery());
        if (activeLocale != null)
        {
            _logger.LogDebug("Found locale {Locale}", activeLocale.IETFLanguageTag);
        }
        else
        {
            _logger.LogDebug("No locale detected");
        }

        return activeLocale;
    }
}
