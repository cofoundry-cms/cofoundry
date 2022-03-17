using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

/// <remarks>
/// This could be nested under the page versions api, but it seemed silly to have to specify
/// all the route paramters when just a pageVersionBlockId would do. To achieve a hierarchical route 
/// as well we could sub-class this type and create two versions with diffent route constraints. See
/// http://stackoverflow.com/a/24969829/716689 for more info about route inheritance.
/// </remarks>
public class PageVersionRegionBlocksApiController : BaseAdminApiController
{
    private readonly IApiResponseHelper _apiResponseHelper;

    public PageVersionRegionBlocksApiController(
        IApiResponseHelper apiResponseHelper
        )
    {
        _apiResponseHelper = apiResponseHelper;
    }

    public async Task<JsonResult> Get(int pageVersionBlockId, PageVersionRegionBlocksActionDataType dataType = PageVersionRegionBlocksActionDataType.RenderDetails)
    {
        if (dataType == PageVersionRegionBlocksActionDataType.UpdateCommand)
        {
            var updateCommandQuery = new GetPatchableCommandByIdQuery<UpdatePageVersionBlockCommand>(pageVersionBlockId);
            return await _apiResponseHelper.RunQueryAsync(updateCommandQuery);
        }

        var query = new GetPageVersionBlockRenderDetailsByIdQuery()
        {
            PageVersionBlockId = pageVersionBlockId,
            PublishStatus = PublishStatusQuery.Latest
        };
        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public Task<JsonResult> Post([ModelBinder(BinderType = typeof(PageVersionBlockDataModelCommandModelBinder))] AddPageVersionBlockCommand command)
    {
        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> Put(int PageVersionBlockId, [ModelBinder(BinderType = typeof(PageVersionBlockDataModelCommandModelBinder))] UpdatePageVersionBlockCommand command)
    {
        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> Delete(int pageVersionBlockId)
    {
        var command = new DeletePageVersionBlockCommand() { PageVersionBlockId = pageVersionBlockId };

        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> MoveUp(int pageVersionBlockId)
    {
        var command = new MovePageVersionBlockCommand();
        command.PageVersionBlockId = pageVersionBlockId;
        command.Direction = OrderedItemMoveDirection.Up;

        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> MoveDown(int pageVersionBlockId)
    {
        var command = new MovePageVersionBlockCommand();
        command.PageVersionBlockId = pageVersionBlockId;
        command.Direction = OrderedItemMoveDirection.Down;

        return _apiResponseHelper.RunCommandAsync(command);
    }
}
