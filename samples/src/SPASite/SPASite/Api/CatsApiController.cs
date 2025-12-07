namespace SPASite;

[Route("api/cats")]
[AutoValidateAntiforgeryToken]
public class CatsApiController : ControllerBase
{
    private readonly IApiResponseHelper _apiResponseHelper;

    public CatsApiController(
        IApiResponseHelper apiResponseHelper
        )
    {
        _apiResponseHelper = apiResponseHelper;
    }

    [HttpGet("")]
    public async Task<JsonResult> Get([FromQuery] SearchCatSummariesQuery query)
    {
        query ??= new SearchCatSummariesQuery();

        return await _apiResponseHelper.RunQueryAsync(query);
    }

    [HttpGet("{catId:int}")]
    public async Task<JsonResult> Get(int catId)
    {
        var query = new GetCatDetailsByIdQuery(catId);

        return await _apiResponseHelper.RunQueryAsync(query);
    }

    /// <summary>
    /// Note that here we use the standard Authorize attribute to restrict
    /// access to this endpoint because you need to be logged in to 'like' a 
    /// cat
    /// </summary>
    [AuthorizeUserArea(MemberUserArea.Code)]
    [HttpPost("{catId:int}/likes")]
    public async Task<JsonResult> Like(int catId)
    {
        var command = new SetCatLikedCommand()
        {
            CatId = catId,
            IsLiked = true
        };

        // IApiResponseHelper will validate the command and permissions before executing it
        // and return any validation errors in a formatted data object
        return await _apiResponseHelper.RunCommandAsync(command);
    }

    [AuthorizeUserArea(MemberUserArea.Code)]
    [HttpDelete("{catId:int}/likes")]
    public async Task<JsonResult> UnLike(int catId)
    {
        var command = new SetCatLikedCommand()
        {
            CatId = catId
        };

        return await _apiResponseHelper.RunCommandAsync(command);
    }
}
