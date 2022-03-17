using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class DocumentsApiController : BaseAdminApiController
{
    private readonly IDomainRepository _domainRepository;
    private readonly IApiResponseHelper _apiResponseHelper;

    public DocumentsApiController(
        IDomainRepository domainRepository,
        IApiResponseHelper apiResponseHelper
        )
    {
        _domainRepository = domainRepository;
        _apiResponseHelper = apiResponseHelper;
    }

    public async Task<JsonResult> Get(
        [FromQuery] SearchDocumentAssetSummariesQuery query,
        [FromQuery] GetDocumentAssetRenderDetailsByIdRangeQuery rangeQuery
        )
    {
        if (rangeQuery != null && rangeQuery.DocumentAssetIds != null)
        {
            return await _apiResponseHelper.RunWithResultAsync(async () =>
            {
                return await _domainRepository
                    .WithQuery(rangeQuery)
                    .FilterAndOrderByKeys(rangeQuery.DocumentAssetIds)
                    .ExecuteAsync();
            });
        }

        if (query == null) query = new SearchDocumentAssetSummariesQuery();
        ApiPagingHelper.SetDefaultBounds(query);

        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public async Task<JsonResult> GetById(int documentAssetId)
    {
        var query = new GetDocumentAssetDetailsByIdQuery(documentAssetId);
        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public Task<JsonResult> Post(AddDocumentAssetCommand command, IFormFile file)
    {
        if (file != null)
        {
            command.File = new FormFileSource(file);
        }
        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> Put(int documentAssetId, UpdateDocumentAssetCommand command, IFormFile file)
    {
        if (file != null)
        {
            command.File = new FormFileSource(file);
        }

        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> Delete(int documentAssetId)
    {
        var command = new DeleteDocumentAssetCommand();
        command.DocumentAssetId = documentAssetId;

        return _apiResponseHelper.RunCommandAsync(command);
    }
}
