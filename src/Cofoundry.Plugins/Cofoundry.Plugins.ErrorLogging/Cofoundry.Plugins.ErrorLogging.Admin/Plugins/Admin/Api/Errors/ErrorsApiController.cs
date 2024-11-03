using Cofoundry.Domain.CQS;
using Cofoundry.Plugins.ErrorLogging.Domain;
using Cofoundry.Web;
using Cofoundry.Web.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Plugins.ErrorLogging.Admin;

public class ErrorsApiController : BaseAdminApiController
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly IApiResponseHelper _apiResponseHelper;

    public ErrorsApiController(
        IQueryExecutor queryExecutor,
        IApiResponseHelper apiResponseHelper
        )
    {
        _queryExecutor = queryExecutor;
        _apiResponseHelper = apiResponseHelper;
    }

    public async Task<IActionResult> Get([FromQuery] SearchErrorSummariesQuery query)
    {
        query ??= new SearchErrorSummariesQuery();

        var results = await _queryExecutor.ExecuteAsync(query);
        return _apiResponseHelper.SimpleQueryResponse(results);
    }

    public async Task<IActionResult> GetById(int errorId)
    {
        var query = new GetErrorDetailsByIdQuery(errorId);
        var result = await _queryExecutor.ExecuteAsync(query);

        return _apiResponseHelper.SimpleQueryResponse(result);
    }
}
