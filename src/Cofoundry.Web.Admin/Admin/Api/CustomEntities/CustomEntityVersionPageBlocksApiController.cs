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
    [AdminApiRoute("custom-entity-version-page-blocks")]
    public class CustomEntityVersionPageBlocksApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{customEntityVersionPageBlockId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly ICommandExecutor _commandExecutor;

        #endregion

        #region constructor

        public CustomEntityVersionPageBlocksApiController(
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
        [Route(ID_ROUTE)]
        public async Task<IActionResult> Get(int customEntityVersionPageBlockId, CustomEntityVersionPageBlocksActionDataType dataType = CustomEntityVersionPageBlocksActionDataType.RenderDetails)
        {
            if (dataType == CustomEntityVersionPageBlocksActionDataType.UpdateCommand)
            {
                return await GetById<UpdateCustomEntityVersionPageBlockCommand>(customEntityVersionPageBlockId);
            }

            var query = new GetCustomEntityVersionPageBlockRenderDetailsByIdQuery() { CustomEntityVersionPageBlockId = customEntityVersionPageBlockId, WorkFlowStatus = WorkFlowStatusQuery.Latest };
            var results = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        private async Task<IActionResult> GetById<T>(int customEntityVersionPageBlockId)
        {
            var results = await _queryExecutor.GetByIdAsync<T>(customEntityVersionPageBlockId);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
        
        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post([ModelBinder(BinderType = typeof(PageVersionBlockDataModelCommandModelBinder))] AddCustomEntityVersionPageBlockCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut(ID_ROUTE)]
        public async Task<IActionResult> Put(int customEntityVersionPageBlockId, [ModelBinder(BinderType = typeof(PageVersionBlockDataModelCommandModelBinder))] UpdateCustomEntityVersionPageBlockCommand command)
        {
            command.CustomEntityVersionPageBlockId = customEntityVersionPageBlockId;
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete(ID_ROUTE)]
        public async Task<IActionResult> Delete(int customEntityVersionPageBlockId)
        {
            var command = new DeleteCustomEntityVersionPageBlockCommand() { CustomEntityVersionPageBlockId = customEntityVersionPageBlockId };

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut(ID_ROUTE + "/move-up")]
        public async Task<IActionResult> MoveUp(int customEntityVersionPageBlockId)
        {
            var command = new MoveCustomEntityVersionPageBlockCommand();
            command.CustomEntityVersionPageBlockId = customEntityVersionPageBlockId;
            command.Direction = OrderedItemMoveDirection.Up;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut(ID_ROUTE + "/move-down")]
        public async Task<IActionResult> MoveDown(int customEntityVersionPageBlockId)
        {
            var command = new MoveCustomEntityVersionPageBlockCommand();
            command.CustomEntityVersionPageBlockId = customEntityVersionPageBlockId;
            command.Direction = OrderedItemMoveDirection.Down;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }
        
        #endregion

        #endregion
    }
}
