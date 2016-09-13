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
    /// <summary>
    /// This api should match the PageVersionSectionModulesApiController endpoint signatures,
    /// since whatever we can do with a page module we can also do with a custom entity module.
    /// </summary>
    [AdminApiRoutePrefix("custom-entity-version-page-modules")]
    public class CustomEntityVersionPageModulesApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{customEntityVersionPageModuleId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly ApiResponseHelper _apiResponseHelper;
        private readonly ICommandExecutor _commandExecutor;

        #endregion

        #region constructor

        public CustomEntityVersionPageModulesApiController(
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
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Get(int customEntityVersionPageModuleId, CustomEntityVersionPageModulesActionDataType dataType = CustomEntityVersionPageModulesActionDataType.RenderDetails)
        {
            if (dataType == CustomEntityVersionPageModulesActionDataType.UpdateCommand)
            {
                return await GetById<UpdateCustomEntityVersionPageModuleCommand>(customEntityVersionPageModuleId);
            }

            var query = new GetCustomEntityVersionPageModuleRenderDetailsByIdQuery() { CustomEntityVersionPageModuleId = customEntityVersionPageModuleId, WorkFlowStatus = WorkFlowStatusQuery.Latest };
            var results = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        private async Task<IHttpActionResult> GetById<T>(int customEntityVersionPageModuleId)
        {
            var results = await _queryExecutor.GetByIdAsync<T>(customEntityVersionPageModuleId);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }


        #endregion

        #region commands

        [HttpPost]
        [Route()]
        public async Task<IHttpActionResult> Post([ModelBinder(typeof(PageVersionModuleDataModelCommandModelBinder))] AddCustomEntityVersionPageModuleCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Put(int customEntityVersionPageModuleId, [ModelBinder(typeof(PageVersionModuleDataModelCommandModelBinder))] UpdateCustomEntityVersionPageModuleCommand command)
        {
            command.CustomEntityVersionPageModuleId = customEntityVersionPageModuleId;
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Delete(int customEntityVersionPageModuleId)
        {
            var command = new DeleteCustomEntityVersionPageModuleCommand() { CustomEntityVersionPageModuleId = customEntityVersionPageModuleId };

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut]
        [Route(ID_ROUTE + "/move-up")]
        public async Task<IHttpActionResult> MoveUp(int customEntityVersionPageModuleId)
        {
            var command = new MoveCustomEntityVersionPageModuleCommand();
            command.CustomEntityVersionPageModuleId = customEntityVersionPageModuleId;
            command.Direction = OrderedItemMoveDirection.Up;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut]
        [Route(ID_ROUTE + "/move-down")]
        public async Task<IHttpActionResult> MoveDown(int customEntityVersionPageModuleId)
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
