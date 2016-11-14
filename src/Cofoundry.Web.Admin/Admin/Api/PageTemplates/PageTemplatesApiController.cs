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
    [AdminApiRoutePrefix("page-templates")]
    public class PageTemplatesApiController : BaseAdminApiController
    {
        #region private member variables
        
        private const string ID_ROUTE = "{id:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly ApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public PageTemplatesApiController(
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
        public async Task<IHttpActionResult> Get([FromUri] SearchPageTemplateSummariesQuery query)
        {
            if (query == null) query = new SearchPageTemplateSummariesQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Get(int id)
        {
            var result = await _queryExecutor.GetByIdAsync<PageTemplateDetails>(id);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #endregion
    }
}