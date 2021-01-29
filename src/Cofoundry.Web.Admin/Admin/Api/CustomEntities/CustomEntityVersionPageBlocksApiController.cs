using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// This api should match the PageVersionRegionBlocksApiController endpoint signatures,
    /// since whatever we can do with a page block we can also do with a custom entity block.
    /// </summary>
    public class CustomEntityVersionPageBlocksApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        public CustomEntityVersionPageBlocksApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #region queries

        public async Task<JsonResult> Get(int customEntityVersionPageBlockId, CustomEntityVersionPageBlocksActionDataType dataType = CustomEntityVersionPageBlocksActionDataType.RenderDetails)
        {
            if (dataType == CustomEntityVersionPageBlocksActionDataType.UpdateCommand)
            {
                var updateCommandQuery = new GetUpdateCommandByIdQuery<UpdateCustomEntityVersionPageBlockCommand>(customEntityVersionPageBlockId);
                var updateCommandResult = await _queryExecutor.ExecuteAsync(updateCommandQuery);

                return _apiResponseHelper.SimpleQueryResponse(updateCommandResult);
            }

            var query = new GetCustomEntityVersionPageBlockRenderDetailsByIdQuery() { CustomEntityVersionPageBlockId = customEntityVersionPageBlockId, PublishStatus = PublishStatusQuery.Latest };
            var results = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(results);
        }
        
        #endregion

        #region commands

        public Task<JsonResult> Post([ModelBinder(BinderType = typeof(PageVersionBlockDataModelCommandModelBinder))] AddCustomEntityVersionPageBlockCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Put(int customEntityVersionPageBlockId, [ModelBinder(BinderType = typeof(PageVersionBlockDataModelCommandModelBinder))] UpdateCustomEntityVersionPageBlockCommand command)
        {
            command.CustomEntityVersionPageBlockId = customEntityVersionPageBlockId;
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Delete(int customEntityVersionPageBlockId)
        {
            var command = new DeleteCustomEntityVersionPageBlockCommand() { CustomEntityVersionPageBlockId = customEntityVersionPageBlockId };

            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> MoveUp(int customEntityVersionPageBlockId)
        {
            var command = new MoveCustomEntityVersionPageBlockCommand();
            command.CustomEntityVersionPageBlockId = customEntityVersionPageBlockId;
            command.Direction = OrderedItemMoveDirection.Up;

            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> MoveDown(int customEntityVersionPageBlockId)
        {
            var command = new MoveCustomEntityVersionPageBlockCommand();
            command.CustomEntityVersionPageBlockId = customEntityVersionPageBlockId;
            command.Direction = OrderedItemMoveDirection.Down;

            return _apiResponseHelper.RunCommandAsync(command);
        }
        
        #endregion
    }
}
