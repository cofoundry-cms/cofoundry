using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    public class PageVersionsApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly ICommandExecutor _commandExecutor;

        public PageVersionsApiController(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
            _commandExecutor = commandExecutor;
        }

        #region queries

        public async Task<IActionResult> Get(int pageId, GetPageVersionSummariesByPageIdQuery query)
        {
            if (query == null) query = new GetPageVersionSummariesByPageIdQuery();
            query.PageId = pageId;
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddPageDraftVersionCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch]
        public async Task<IActionResult> PatchDraft(int pageId, [FromBody] IDelta<UpdatePageDraftVersionCommand> delta)
        {
            // Custom patching because we may need to create a draft version first
            var query = new GetUpdateCommandByIdQuery<UpdatePageDraftVersionCommand>(pageId);
            var command = await _queryExecutor.ExecuteAsync(query);

            if (command == null)
            {
                var createDraftCommand = new AddPageDraftVersionCommand();
                createDraftCommand.PageId = pageId;
                await _commandExecutor.ExecuteAsync(createDraftCommand);
                command = await _queryExecutor.ExecuteAsync(query);
            }

            delta.Patch(command);

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDraft(int pageId)
        {
            var command = new DeletePageDraftVersionCommand() { PageId = pageId };

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch]
        public async Task<IActionResult> Publish(int pageId)
        {
            var command = new PublishPageCommand() { PageId = pageId };
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch]
        public async Task<IActionResult> UnPublish(int pageId)
        {
            var command = new UnPublishPageCommand() { PageId = pageId };
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion
    }
}