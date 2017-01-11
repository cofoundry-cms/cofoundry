using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoutePrefix("pages/{pageId:int}/versions")]
    public class PageVersionsApiController : BaseAdminApiController
    {
        #region private member variables

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly ICommandExecutor _commandExecutor;

        #endregion

        #region constructor

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

        #endregion

        #region routes

        #region queries

        [HttpGet]
        [Route]
        public async Task<IHttpActionResult> Get(int pageId)
        {
            var query = new GetPageVersionDetailsByPageIdQuery() { PageId = pageId };

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        #endregion

        #region commands

        [HttpPost]
        [Route()]
        public async Task<IHttpActionResult> Post(AddPageDraftVersionCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch]
        [Route("draft")]
        public async Task<IHttpActionResult> PatchDraft(int pageId, Delta<UpdatePageDraftVersionCommand> delta)
        {
            // Custom patching because we may need to create a draft version first
            var command = await _queryExecutor.GetByIdAsync<UpdatePageDraftVersionCommand>(pageId);
            if (command == null)
            {
                var createDraftCommand = new AddPageDraftVersionCommand();
                createDraftCommand.PageId = pageId;
                await _commandExecutor.ExecuteAsync(createDraftCommand);
                command = await _queryExecutor.GetByIdAsync<UpdatePageDraftVersionCommand>(pageId);
            }

            delta.Patch(command);

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete]
        [Route("draft")]
        public async Task<IHttpActionResult> DeleteDraft(int pageId)
        {
            var command = new DeletePageDraftVersionCommand() { PageId = pageId };

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch]
        [Route("draft/publish")]
        public async Task<IHttpActionResult> Publish(int pageId)
        {
            var command = new PublishPageCommand() { PageId = pageId };
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch]
        [Route("published/unpublish")]
        public async Task<IHttpActionResult> UnPublish(int pageId)
        {
            var command = new UnPublishPageCommand() { PageId = pageId };
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}