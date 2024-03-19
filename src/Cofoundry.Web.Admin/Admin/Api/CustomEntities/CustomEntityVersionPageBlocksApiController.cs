﻿using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

/// <summary>
/// This api should match the PageVersionRegionBlocksApiController endpoint signatures,
/// since whatever we can do with a page block we can also do with a custom entity block.
/// </summary>
public class CustomEntityVersionPageBlocksApiController : BaseAdminApiController
{
    private readonly IApiResponseHelper _apiResponseHelper;

    public CustomEntityVersionPageBlocksApiController(
        IApiResponseHelper apiResponseHelper
        )
    {
        _apiResponseHelper = apiResponseHelper;
    }

    public async Task<JsonResult> Get(int customEntityVersionPageBlockId, CustomEntityVersionPageBlocksActionDataType dataType = CustomEntityVersionPageBlocksActionDataType.RenderDetails)
    {
        if (dataType == CustomEntityVersionPageBlocksActionDataType.UpdateCommand)
        {
            var updateCommandQuery = new GetPatchableCommandByIdQuery<UpdateCustomEntityVersionPageBlockCommand>(customEntityVersionPageBlockId);
            return await _apiResponseHelper.RunQueryAsync(updateCommandQuery);
        }

        var query = new GetCustomEntityVersionPageBlockRenderDetailsByIdQuery()
        {
            CustomEntityVersionPageBlockId = customEntityVersionPageBlockId,
            PublishStatus = PublishStatusQuery.Latest
        };

        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public Task<JsonResult> Post([ModelBinder(BinderType = typeof(PageVersionBlockDataModelCommandModelBinder))] AddCustomEntityVersionPageBlockCommand command)
    {
        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> Put(int customEntityVersionPageBlockId, [ModelBinder(BinderType = typeof(PageVersionBlockDataModelCommandModelBinder))] UpdateCustomEntityVersionPageBlockCommand command)
    {
        command.CustomEntityVersionPageBlockId = customEntityVersionPageBlockId;
        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> Delete(int customEntityVersionPageBlockId)
    {
        var command = new DeleteCustomEntityVersionPageBlockCommand() { CustomEntityVersionPageBlockId = customEntityVersionPageBlockId };

        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> MoveUp(int customEntityVersionPageBlockId)
    {
        var command = new MoveCustomEntityVersionPageBlockCommand
        {
            CustomEntityVersionPageBlockId = customEntityVersionPageBlockId,
            Direction = OrderedItemMoveDirection.Up
        };

        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> MoveDown(int customEntityVersionPageBlockId)
    {
        var command = new MoveCustomEntityVersionPageBlockCommand
        {
            CustomEntityVersionPageBlockId = customEntityVersionPageBlockId,
            Direction = OrderedItemMoveDirection.Down
        };

        return _apiResponseHelper.RunCommandAsync(command);
    }
}
