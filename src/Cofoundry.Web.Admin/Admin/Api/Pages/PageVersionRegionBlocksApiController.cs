using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    /// <remarks>
    /// This could be nested under the page versions api, but it seemed silly to have to specify
    /// all the route paramters when just a pageVersionBlockId would do. To achieve a hierarchical route 
    /// as well we could sub-class this type and create two versions with diffent route constraints. See
    /// http://stackoverflow.com/a/24969829/716689 for more info about route inheritance.
    /// </remarks>
    public class PageVersionRegionBlocksApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;
        private readonly ICommandExecutor _commandExecutor;

        public PageVersionRegionBlocksApiController(
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

        public async Task<JsonResult> Get(int pageVersionBlockId, PageVersionRegionBlocksActionDataType dataType = PageVersionRegionBlocksActionDataType.RenderDetails)
        {
            if (dataType == PageVersionRegionBlocksActionDataType.UpdateCommand)
            {
                var updateCommandQuery = new GetUpdateCommandByIdQuery<UpdatePageVersionBlockCommand>(pageVersionBlockId);
                var updateCommandResult = await _queryExecutor.ExecuteAsync(updateCommandQuery);

                return _apiResponseHelper.SimpleQueryResponse(updateCommandResult);
            }
            
            var query = new GetPageVersionBlockRenderDetailsByIdQuery() { PageVersionBlockId = pageVersionBlockId, PublishStatus = PublishStatusQuery.Latest };
            var results = await _queryExecutor.ExecuteAsync(query);
            
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        #endregion

        #region commands

        public Task<JsonResult> Post([ModelBinder(BinderType = typeof(PageVersionBlockDataModelCommandModelBinder))] AddPageVersionBlockCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Put(int PageVersionBlockId, [ModelBinder(BinderType = typeof(PageVersionBlockDataModelCommandModelBinder))] UpdatePageVersionBlockCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Delete(int pageVersionBlockId)
        {
            var command = new DeletePageVersionBlockCommand() { PageVersionBlockId = pageVersionBlockId };

            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> MoveUp(int pageVersionBlockId)
        {
            var command = new MovePageVersionBlockCommand();
            command.PageVersionBlockId = pageVersionBlockId;
            command.Direction = OrderedItemMoveDirection.Up;

            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> MoveDown(int pageVersionBlockId)
        {
            var command = new MovePageVersionBlockCommand();
            command.PageVersionBlockId = pageVersionBlockId;
            command.Direction = OrderedItemMoveDirection.Down;

            return _apiResponseHelper.RunCommandAsync(command);
        }
        
        #endregion
    }
}
