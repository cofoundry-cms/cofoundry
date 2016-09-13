using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoutePrefix("pages")]
    public class PagesApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{pageId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly ApiResponseHelper _apiResponseHelper;
        #endregion

        #region constructor

        public PagesApiController(
            IQueryExecutor queryExecutor,
            ApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #endregion

        #region routes

        #region queries

        [HttpGet]
        [Route]
        public async Task<IHttpActionResult> Get([FromUri] SearchPageSummariesQuery query)
        {
            if (query == null) query = new SearchPageSummariesQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Get(int pageId)
        {
            var result = await _queryExecutor.GetByIdAsync<PageDetails>(pageId);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        [Route()]
        public async Task<IHttpActionResult> Post(AddPageCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPatch]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Patch(int pageId, Delta<UpdatePageCommand> delta)
        {
            return await _apiResponseHelper.RunCommandAsync(this, pageId, delta);
        }

        [HttpPut]
        [Route(ID_ROUTE + "/url")]
        public async Task<IHttpActionResult> PutPageUrl(int pageId, UpdatePageUrlCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Delete(int pageId)
        {
            var command = new DeletePageCommand();
            command.PageId = pageId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPost]
        [Route(ID_ROUTE + "/duplicate")]
        public async Task<IHttpActionResult> Post(DuplicatePageCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}