using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class CustomEntitiesApiController : BaseAdminApiController
{
    private readonly IDomainRepository _domainRepository;
    private readonly IApiResponseHelper _apiResponseHelper;

    public CustomEntitiesApiController(
        IDomainRepository domainRepository,
        IApiResponseHelper apiResponseHelper
        )
    {
        _domainRepository = domainRepository;
        _apiResponseHelper = apiResponseHelper;
    }

    public async Task<JsonResult> Get([FromQuery] SearchCustomEntitySummariesQuery query, [FromQuery] GetCustomEntitySummariesByIdRangeQuery rangeQuery)
    {
        if (rangeQuery != null && rangeQuery.CustomEntityIds != null)
        {
            return await _apiResponseHelper.RunWithResultAsync(async () =>
            {
                return await _domainRepository
                    .WithQuery(rangeQuery)
                    .FilterAndOrderByKeys(rangeQuery.CustomEntityIds)
                    .ExecuteAsync();
            });
        }

        if (query == null) query = new SearchCustomEntitySummariesQuery();
        ApiPagingHelper.SetDefaultBounds(query);

        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public async Task<JsonResult> GetById(int customEntityId)
    {
        var query = new GetCustomEntityDetailsByIdQuery(customEntityId);
        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public Task<JsonResult> Post([ModelBinder(BinderType = typeof(CustomEntityDataModelCommandModelBinder))] AddCustomEntityCommand command)
    {
        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> PutOrdering([FromBody] ReOrderCustomEntitiesCommand command)
    {
        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> PutCustomEntityUrl(int customEntityId, [FromBody] UpdateCustomEntityUrlCommand command)
    {
        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> Delete(int customEntityId)
    {
        var command = new DeleteCustomEntityCommand();
        command.CustomEntityId = customEntityId;

        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> PostDuplicate([FromBody] DuplicateCustomEntityCommand command)
    {
        return _apiResponseHelper.RunCommandAsync(command);
    }
}
