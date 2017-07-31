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
    /// This api should match the PageVersionSectionModulesApiController endpoint signatures,
    /// since whatever we can do with a page module we can also do with a custom entity module.
    /// </summary>
    [AdminApiRoute("custom-entity-version-page-modules")]
    public class CustomEntityVersionPageModulesApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{customEntityVersionPageModuleId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly ICommandExecutor _commandExecutor;

        #endregion

        #region constructor

        public CustomEntityVersionPageModulesApiController(
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
        public async Task<IActionResult> Get(int customEntityVersionPageModuleId, CustomEntityVersionPageModulesActionDataType dataType = CustomEntityVersionPageModulesActionDataType.RenderDetails)
        {
            if (dataType == CustomEntityVersionPageModulesActionDataType.UpdateCommand)
            {
                return await GetById<UpdateCustomEntityVersionPageModuleCommand>(customEntityVersionPageModuleId);
            }

            var query = new GetCustomEntityVersionPageModuleRenderDetailsByIdQuery() { CustomEntityVersionPageModuleId = customEntityVersionPageModuleId, WorkFlowStatus = WorkFlowStatusQuery.Latest };
            var results = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        private async Task<IActionResult> GetById<T>(int customEntityVersionPageModuleId)
        {
            var results = await _queryExecutor.GetByIdAsync<T>(customEntityVersionPageModuleId);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }


        #endregion

        #region commands

        [HttpPost]
        public async Task<IActionResult> Post([ModelBinder(BinderType = typeof(PageVersionModuleDataModelCommandModelBinder))] AddCustomEntityVersionPageModuleCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut(ID_ROUTE)]
        public async Task<IActionResult> Put(int customEntityVersionPageModuleId, [ModelBinder(BinderType = typeof(PageVersionModuleDataModelCommandModelBinder))] UpdateCustomEntityVersionPageModuleCommand command)
        {
            command.CustomEntityVersionPageModuleId = customEntityVersionPageModuleId;
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete(ID_ROUTE)]
        public async Task<IActionResult> Delete(int customEntityVersionPageModuleId)
        {
            var command = new DeleteCustomEntityVersionPageModuleCommand() { CustomEntityVersionPageModuleId = customEntityVersionPageModuleId };

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut(ID_ROUTE + "/move-up")]
        public async Task<IActionResult> MoveUp(int customEntityVersionPageModuleId)
        {
            var command = new MoveCustomEntityVersionPageModuleCommand();
            command.CustomEntityVersionPageModuleId = customEntityVersionPageModuleId;
            command.Direction = OrderedItemMoveDirection.Up;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut(ID_ROUTE + "/move-down")]
        public async Task<IActionResult> MoveDown(int customEntityVersionPageModuleId)
        {
            var command = new MoveCustomEntityVersionPageModuleCommand();
            command.CustomEntityVersionPageModuleId = customEntityVersionPageModuleId;
            command.Direction = OrderedItemMoveDirection.Down;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }
        
        #endregion

        #endregion
    }
}
