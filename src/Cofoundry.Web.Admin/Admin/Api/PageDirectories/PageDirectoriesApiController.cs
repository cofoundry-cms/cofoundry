using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    public class PageDirectoriesApiController : BaseAdminApiController
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        public PageDirectoriesApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #region queries

        public async Task<JsonResult> Get()
        {
            var results = await _queryExecutor.ExecuteAsync(new GetAllPageDirectoryRoutesQuery());
            return _apiResponseHelper.SimpleQueryResponse(results);
        }

        public async Task<JsonResult> GetTree()
        {
            var query = new GetPageDirectoryTreeQuery();
            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(results);
        }
        
        public async Task<JsonResult> GetById(int pageDirectoryId)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetPageDirectoryNodeByIdQuery(pageDirectoryId));
            return _apiResponseHelper.SimpleQueryResponse(result);
        }

        #endregion

        #region commands

        public Task<JsonResult> Post([FromBody] AddPageDirectoryCommand command)
        {
            return _apiResponseHelper.RunCommandAsync(command);
        }

        public Task<JsonResult> Patch(int pageDirectoryId, [FromBody] IDelta<UpdatePageDirectoryCommand> delta)
        {
            return _apiResponseHelper.RunCommandAsync(pageDirectoryId, delta);
        }

        public Task<JsonResult> Delete(int pageDirectoryId)
        {
            var command = new DeletePageDirectoryCommand();
            command.PageDirectoryId = pageDirectoryId;

            return _apiResponseHelper.RunCommandAsync(command);
        }

        #endregion
    }
}