namespace SPASite;

[Route("api/features")]
public class FeaturesApiController : ControllerBase
{
    private readonly IApiResponseHelper _apiResponseHelper;

    public FeaturesApiController(IApiResponseHelper apiResponseHelper)
    {
        _apiResponseHelper = apiResponseHelper;
    }

    [HttpGet("")]
    public async Task<JsonResult> Get()
    {
        var query = new GetAllFeaturesQuery();

        return await _apiResponseHelper.RunQueryAsync(query);
    }
}
