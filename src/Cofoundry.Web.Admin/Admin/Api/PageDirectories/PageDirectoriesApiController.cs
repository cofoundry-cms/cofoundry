using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class PageDirectoriesApiController : BaseAdminApiController
{
    private readonly IApiResponseHelper _apiResponseHelper;

    public PageDirectoriesApiController(
        IApiResponseHelper apiResponseHelper
        )
    {
        _apiResponseHelper = apiResponseHelper;
    }

    public async Task<JsonResult> Get()
    {
        var query = new GetAllPageDirectoryRoutesQuery();
        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public async Task<JsonResult> GetTree()
    {
        var query = new GetPageDirectoryTreeQuery();
        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public async Task<JsonResult> GetById(int pageDirectoryId)
    {
        var query = new GetPageDirectoryNodeByIdQuery(pageDirectoryId);
        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public Task<JsonResult> Post([FromBody] AddPageDirectoryCommand command)
    {
        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> Patch(int pageDirectoryId, [FromBody] IDelta<UpdatePageDirectoryCommand> delta)
    {
        return _apiResponseHelper.RunCommandAsync(pageDirectoryId, delta);
    }

    public Task<JsonResult> PutUrl(int pageId, [FromBody] UpdatePageDirectoryUrlCommand command)
    {
        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> Delete(int pageDirectoryId)
    {
        var command = new DeletePageDirectoryCommand();
        command.PageDirectoryId = pageDirectoryId;

        return _apiResponseHelper.RunCommandAsync(command);
    }
}
