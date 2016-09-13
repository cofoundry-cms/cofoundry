using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoutePrefix("custom-entities/{customEntityId:int}/versions")]
    public class CustomEntityVersionsApiController : BaseAdminApiController
    {
        #region private member variables

        private readonly IQueryExecutor _queryExecutor;
        private readonly ApiResponseHelper _apiResponseHelper;
        private readonly ICommandExecutor _commandExecutor;

        #endregion

        #region constructor

        public CustomEntityVersionsApiController(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            ApiResponseHelper apiResponseHelper
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
        public async Task<IHttpActionResult> Get(int customEntityId)
        {
            var query = new GetCustomEntityVersionSummariesByCustomEntityIdQuery() { CustomEntityId = customEntityId };

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        #endregion

        #region commands

        [HttpPost]
        [Route()]
        public async Task<IHttpActionResult> Post(AddCustomEntityDraftVersionCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut]
        [Route("draft")]
        public async Task<IHttpActionResult> PutDraft(int customEntityId, [ModelBinder(typeof(CustomEntityDataModelCommandModelBinder))] UpdateCustomEntityDraftVersionCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete]
        [Route("draft")]
        public async Task<IHttpActionResult> DeleteDraft(int customEntityId)
        {
            var command = new DeleteCustomEntityDraftVersionCommand() { CustomEntityId = customEntityId };

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch]
        [Route("draft/publish")]
        public async Task<IHttpActionResult> Publish(int customEntityId)
        {
            var command = new PublishCustomEntityCommand() { CustomEntityId = customEntityId };
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch]
        [Route("published/unpublish")]
        public async Task<IHttpActionResult> UnPublish(int customEntityId)
        {
            var command = new UnPublishCustomEntityCommand() { CustomEntityId = customEntityId };
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}