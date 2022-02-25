using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    public class PageVersionsApiController : BaseAdminApiController
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IApiResponseHelper _apiResponseHelper;

        public PageVersionsApiController(
            IDomainRepository domainRepository,
            IApiResponseHelper apiResponseHelper
            )
        {
            _domainRepository = domainRepository;
            _apiResponseHelper = apiResponseHelper;
        }

        public async Task<JsonResult> Get(int pageId, GetPageVersionSummariesByPageIdQuery query)
        {
            if (query == null) query = new GetPageVersionSummariesByPageIdQuery();
            query.PageId = pageId;
            ApiPagingHelper.SetDefaultBounds(query);

            return await _apiResponseHelper.RunQueryAsync(query);
        }

        public Task<JsonResult> Post([FromBody] AddPageDraftVersionCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public async Task<JsonResult> PatchDraft(int pageId, [FromBody] IDelta<UpdatePageDraftVersionCommand> delta)
        {
            // Custom patching because we may need to create a draft version first
            var query = new GetPatchableCommandByIdQuery<UpdatePageDraftVersionCommand>(pageId);
            var command = await _domainRepository.ExecuteQueryAsync(query);

            if (command == null)
            {
                var createDraftCommand = new AddPageDraftVersionCommand();
                createDraftCommand.PageId = pageId;
                await _domainRepository.ExecuteCommandAsync(createDraftCommand);
                command = await _domainRepository.ExecuteQueryAsync(query);
            }

            delta.Patch(command);

            return await _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> DeleteDraft(int pageId)
        {
            var command = new DeletePageDraftVersionCommand() { PageId = pageId };
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Publish(int pageId)
        {
            var command = new PublishPageCommand() { PageId = pageId };
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> UnPublish(int pageId)
        {
            var command = new UnPublishPageCommand() { PageId = pageId };
            return _apiResponseHelper.RunCommandAsync(command);
        }
    }
}