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

        public async Task<JsonResult> Get(int pageId, GetPageVersionSummariesByPageIdQuery query)
        {
            if (query == null) query = new GetPageVersionSummariesByPageIdQuery();
            query.PageId = pageId;
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        #endregion

        #region commands

        public Task<JsonResult> Post([FromBody] AddPageDraftVersionCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public async Task<JsonResult> PatchDraft(int pageId, [FromBody] IDelta<UpdatePageDraftVersionCommand> delta)
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

        #endregion
    }
}