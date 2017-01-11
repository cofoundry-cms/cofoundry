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
    /// <remarks>
    /// This could be nested under the page versions api, but it seemed silly to have to specify
    /// all the route paramters when just a pageVersionModuleId would do. To achieve a heirachical route 
    /// as well we could sub-class this type and create two versions with diffent route constraints. See
    /// http://stackoverflow.com/a/24969829/716689 for more info abotu route inheritance.
    /// </remarks>
    [AdminApiRoutePrefix("page-version-section-modules")]
    public class PageVersionSectionModulesApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{pageVersionModuleId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly ICommandExecutor _commandExecutor;

        #endregion

        #region constructor

        public PageVersionSectionModulesApiController(
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
        public async Task<IHttpActionResult> Get(int pageVersionModuleId, PageVersionSectionModulesActionDataType dataType = PageVersionSectionModulesActionDataType.RenderDetails)
        {
            if (dataType == PageVersionSectionModulesActionDataType.UpdateCommand)
            {
                return await GetById<UpdatePageVersionModuleCommand>(pageVersionModuleId);
            }
            
            var query = new GetPageVersionModuleRenderDetailsByIdQuery() { PageVersionModuleId = pageVersionModuleId, WorkFlowStatus = WorkFlowStatusQuery.Latest };
            var results = await _queryExecutor.ExecuteAsync(query);

            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        private async Task<IHttpActionResult> GetById<T>(int pageVersionModuleId)
        {
            var results = await _queryExecutor.GetByIdAsync<T>(pageVersionModuleId);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        #endregion

        #region commands

        [HttpPost]
        [Route()]
        public async Task<IHttpActionResult> Post([ModelBinder(typeof(PageVersionModuleDataModelCommandModelBinder))] AddPageVersionModuleCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Put(int PageVersionModuleId, [ModelBinder(typeof(PageVersionModuleDataModelCommandModelBinder))] UpdatePageVersionModuleCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Delete(int pageVersionModuleId)
        {
            var command = new DeletePageVersionModuleCommand() { PageVersionModuleId = pageVersionModuleId };

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut]
        [Route(ID_ROUTE + "/move-up")]
        public async Task<IHttpActionResult> MoveUp(int pageVersionModuleId)
        {
            var command = new MovePageVersionModuleCommand();
            command.PageVersionModuleId = pageVersionModuleId;
            command.Direction = OrderedItemMoveDirection.Up;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut]
        [Route(ID_ROUTE + "/move-down")]
        public async Task<IHttpActionResult> MoveDown(int pageVersionModuleId)
        {
            var command = new MovePageVersionModuleCommand();
            command.PageVersionModuleId = pageVersionModuleId;
            command.Direction = OrderedItemMoveDirection.Down;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }
        
        #endregion

        #endregion
    }
}
