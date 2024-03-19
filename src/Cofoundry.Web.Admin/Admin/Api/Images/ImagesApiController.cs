﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin;

public class ImagesApiController : BaseAdminApiController
{
    private readonly IDomainRepository _domainRepository;
    private readonly IApiResponseHelper _apiResponseHelper;
    private readonly ImageAssetsSettings _imageAssetsSettings;

    public ImagesApiController(
        IDomainRepository domainRepository,
        IApiResponseHelper apiResponseHelper,
        ImageAssetsSettings imageAssetsSettings
        )
    {
        _domainRepository = domainRepository;
        _apiResponseHelper = apiResponseHelper;
        _imageAssetsSettings = imageAssetsSettings;
    }

    public async Task<JsonResult> Get(
        [FromQuery] SearchImageAssetSummariesQuery? query,
        [FromQuery] GetImageAssetRenderDetailsByIdRangeQuery? rangeQuery
        )
    {
        if (rangeQuery != null && !EnumerableHelper.IsNullOrEmpty(rangeQuery.ImageAssetIds))
        {
            return await _apiResponseHelper.RunWithResultAsync(async () =>
            {
                return await _domainRepository
                    .WithQuery(rangeQuery)
                    .FilterAndOrderByKeys(rangeQuery.ImageAssetIds)
                    .ExecuteAsync();
            });
        }

        query ??= new SearchImageAssetSummariesQuery();
        ApiPagingHelper.SetDefaultBounds(query);

        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public async Task<JsonResult> GetById(int imageAssetId)
    {
        var query = new GetImageAssetDetailsByIdQuery(imageAssetId);
        return await _apiResponseHelper.RunQueryAsync(query);
    }

    public JsonResult GetSettings()
    {
        return _apiResponseHelper.SimpleQueryResponse(new
        {
            _imageAssetsSettings.MaxUploadWidth,
            _imageAssetsSettings.MaxUploadHeight
        });
    }

    public Task<JsonResult> Post(AddImageAssetCommand command, IFormFile file)
    {
        if (file != null)
        {
            command.File = new FormFileSource(file);
        }

        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> Put(int imageAssetId, UpdateImageAssetCommand command, IFormFile file)
    {
        if (file != null)
        {
            command.File = new FormFileSource(file);
        }

        return _apiResponseHelper.RunCommandAsync(command);
    }

    public Task<JsonResult> Delete(int imageAssetId)
    {
        var command = new DeleteImageAssetCommand
        {
            ImageAssetId = imageAssetId
        };

        return _apiResponseHelper.RunCommandAsync(command);
    }
}
