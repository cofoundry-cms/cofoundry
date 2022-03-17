using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class CustomEntityVersionsApiController : BaseAdminApiController
{
    private readonly IApiResponseHelper _apiResponseHelper;

    public CustomEntityVersionsApiController(
        IApiResponseHelper apiResponseHelper
        )
    {
        _apiResponseHelper = apiResponseHelper;
    }

    public async Task<JsonResult> Get(int customEntityId, GetCustomEntityVersionSummariesByCustomEntityIdQuery query)
    {
        if (query == null) query = new GetCustomEntityVersionSummariesByCustomEntityIdQuery();

        query.CustomEntityId = customEntityId;
        ApiPagingHelper.SetDefaultBounds(query);

        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public Task<JsonResult> Post([FromBody] AddCustomEntityDraftVersionCommand command)
    {
        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> PutDraft(int customEntityId, [ModelBinder(BinderType = typeof(CustomEntityDataModelCommandModelBinder))] UpdateCustomEntityDraftVersionCommand command)
    {
        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> DeleteDraft(int customEntityId)
    {
        var command = new DeleteCustomEntityDraftVersionCommand() { CustomEntityId = customEntityId };

        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> Publish(int customEntityId)
    {
        var command = new PublishCustomEntityCommand() { CustomEntityId = customEntityId };
        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> UnPublish(int customEntityId)
    {
        var command = new UnPublishCustomEntityCommand() { CustomEntityId = customEntityId };
        return _apiResponseHelper.RunCommandAsync(command);
    }
}
