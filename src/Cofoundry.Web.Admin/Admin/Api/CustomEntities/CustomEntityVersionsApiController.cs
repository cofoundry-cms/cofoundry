using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoute("custom-entities/{customEntityId:int}/versions")]
    public class CustomEntityVersionsApiController : BaseAdminApiController
    {
        #region private member variables

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly ICommandExecutor _commandExecutor;

        #endregion

        #region constructor

        public CustomEntityVersionsApiController(
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
        public async Task<IActionResult> Get(int customEntityId, GetCustomEntityVersionSummariesByCustomEntityIdQuery query)
        {
            if (query == null) query = new GetCustomEntityVersionSummariesByCustomEntityIdQuery();

            query.CustomEntityId = customEntityId;
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddCustomEntityDraftVersionCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut("draft")]
        public async Task<IActionResult> PutDraft(int customEntityId, [ModelBinder(BinderType = typeof(CustomEntityDataModelCommandModelBinder))] UpdateCustomEntityDraftVersionCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete("draft")]
        public async Task<IActionResult> DeleteDraft(int customEntityId)
        {
            var command = new DeleteCustomEntityDraftVersionCommand() { CustomEntityId = customEntityId };

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch("draft/publish")]
        public async Task<IActionResult> Publish(int customEntityId)
        {
            var command = new PublishCustomEntityCommand() { CustomEntityId = customEntityId };
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch("published/unpublish")]
        public async Task<IActionResult> UnPublish(int customEntityId)
        {
            var command = new UnPublishCustomEntityCommand() { CustomEntityId = customEntityId };
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}