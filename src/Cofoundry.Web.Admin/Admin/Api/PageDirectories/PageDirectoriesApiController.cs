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
        private const string ID_ROUTE = "{pageDirectoryId:int}";

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

        public async Task<IActionResult> Get()
        {
            var results = await _queryExecutor.ExecuteAsync(new GetAllPageDirectoryRoutesQuery());
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        public async Task<IActionResult> GetTree()
        {
            var query = new GetPageDirectoryTreeQuery();
            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }
        
        public async Task<IActionResult> GetById(int pageDirectoryId)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetPageDirectoryNodeByIdQuery(pageDirectoryId));
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        public async Task<IActionResult> Post([FromBody] AddPageDirectoryCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        public async Task<IActionResult> Patch(int pageDirectoryId, [FromBody] IDelta<UpdatePageDirectoryCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, pageDirectoryId, delta);
        }

        public async Task<IActionResult> Delete(int pageDirectoryId)
        {
            var command = new DeletePageDirectoryCommand();
            command.PageDirectoryId = pageDirectoryId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion
    }
}