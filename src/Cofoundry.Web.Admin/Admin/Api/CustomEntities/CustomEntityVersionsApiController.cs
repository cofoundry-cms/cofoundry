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

        public CustomEntityVersionsApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #region queries

        public async Task<JsonResult> Get(int customEntityId, GetCustomEntityVersionSummariesByCustomEntityIdQuery query)
        {
            if (query == null) query = new GetCustomEntityVersionSummariesByCustomEntityIdQuery();

            query.CustomEntityId = customEntityId;
            ApiPagingHelper.SetDefaultBounds(query);

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        #endregion

        #region commands

        public Task<JsonResult> Post([FromBody] AddCustomEntityDraftVersionCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> PutDraft(int customEntityId, [ModelBinder(BinderType = typeof(CustomEntityDataModelCommandModelBinder))] UpdateCustomEntityDraftVersionCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> DeleteDraft(int customEntityId)
        {
            var command = new DeleteCustomEntityDraftVersionCommand() { CustomEntityId = customEntityId };

            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Publish(int customEntityId)
        {
            var command = new PublishCustomEntityCommand() { CustomEntityId = customEntityId };
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> UnPublish(int customEntityId)
        {
            var command = new UnPublishCustomEntityCommand() { CustomEntityId = customEntityId };
            return _apiResponseHelper.RunCommandAsync(command);
        }

        #endregion
    }
}