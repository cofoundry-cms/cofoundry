using Cofoundry.Core;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class PagesApiController : BaseAdminApiController
    {
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly IDomainRepository _domainRepository;

        public PagesApiController(
            IDomainRepository domainRepository,
            IApiResponseHelper apiResponseHelper
            )
        {
            _domainRepository = domainRepository;
            _apiResponseHelper = apiResponseHelper;
        }

        public async Task<JsonResult> Get(
            [FromQuery] SearchPageSummariesQuery query,
            [FromQuery] GetPageSummariesByIdRangeQuery rangeQuery
            )
        {
            if (rangeQuery != null && rangeQuery.PageIds != null)
            {
                return await _apiResponseHelper.RunWithResultAsync(async () =>
                {
                    return _domainRepository
                        .WithQuery(rangeQuery)
                        .FilterAndOrderByKeys(rangeQuery.PageIds)
                        .ExecuteAsync();
                });
            }

            if (query == null) query = new SearchPageSummariesQuery();
            ApiPagingHelper.SetDefaultBounds(query);

            return await _apiResponseHelper.RunQueryAsync(query);
        }

        public async Task<JsonResult> GetById(int pageId)
        {
            var query = new GetPageDetailsByIdQuery(pageId);
            return await _apiResponseHelper.RunQueryAsync(query);
        }

        public Task<JsonResult> Post([FromBody] AddPageCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Patch(int pageId, [FromBody] IDelta<UpdatePageCommand> delta)
        {
            return _apiResponseHelper.RunCommandAsync(pageId, delta);
        }

        public Task<JsonResult> PutPageUrl(int pageId, [FromBody] UpdatePageUrlCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Delete(int pageId)
        {
            var command = new DeletePageCommand();
            command.PageId = pageId;

            return _apiResponseHelper.RunCommandAsync(command);
        }

        public async Task<JsonResult> PostDuplicate([FromBody] DuplicatePageCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(command);
        }
    }
}