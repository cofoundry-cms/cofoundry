using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    public class CustomEntityVersionsApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly ICommandExecutor _commandExecutor;

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

        #region queries

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

        public async Task<IActionResult> Post([FromBody] AddCustomEntityDraftVersionCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        public async Task<IActionResult> PutDraft(int customEntityId, [ModelBinder(BinderType = typeof(CustomEntityDataModelCommandModelBinder))] UpdateCustomEntityDraftVersionCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        public async Task<IActionResult> DeleteDraft(int customEntityId)
        {
            var command = new DeleteCustomEntityDraftVersionCommand() { CustomEntityId = customEntityId };

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        public async Task<IActionResult> Publish(int customEntityId)
        {
            var command = new PublishCustomEntityCommand() { CustomEntityId = customEntityId };
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        public async Task<IActionResult> UnPublish(int customEntityId)
        {
            var command = new UnPublishCustomEntityCommand() { CustomEntityId = customEntityId };
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion
    }
}