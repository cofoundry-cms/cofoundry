namespace SPASite;

[Route("api/breeds")]
public class BreedsApiController : ControllerBase
{
    private readonly IApiResponseHelper _apiResponseHelper;

    public BreedsApiController(
        IApiResponseHelper apiResponseHelper
        )
    {
        _apiResponseHelper = apiResponseHelper;
    }

    [HttpGet("")]
    public async Task<JsonResult> Get()
    {
        var query = new GetAllBreedsQuery();

        return await _apiResponseHelper.RunQueryAsync(query);
    }

    [HttpGet("{breedId:int}")]
    public async Task<JsonResult> Get(int breedId)
    {
        var query = new GetBreedByIdQuery(breedId);

        return await _apiResponseHelper.RunQueryAsync(query);
    }
}