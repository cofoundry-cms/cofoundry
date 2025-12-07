using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;

namespace HangfireSample.Controllers;

public class HomeController : Controller
{
    private readonly IQueryExecutor _queryExecutor;

    public HomeController(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    [Route("")]
    public async Task<IActionResult> Index()
    {
        var query = new SearchCustomEntityRenderSummariesQuery()
        {
            CustomEntityDefinitionCode = ProductCustomEntityDefinition.DefinitionCode,
            PageSize = 10
        };

        var result = await _queryExecutor.ExecuteAsync(query);
        return View(result);
    }
}
