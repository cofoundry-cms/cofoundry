using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoutePrefix("page-module-types")]
    public class PageModuleTypesApiController : BaseAdminApiController
    {
        #region private member variables
        
        private readonly IQueryExecutor _queryExecutor;
        private readonly ApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public PageModuleTypesApiController(
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
        public async Task<IHttpActionResult> Get()
        {
            var results = await _queryExecutor.GetAllAsync<PageModuleTypeSummary>();
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet]
        [Route("{pageModuleTypeId:int}")]
        public async Task<IHttpActionResult> Get(int pageModuleTypeId)
        {
            var results = await _queryExecutor.GetByIdAsync<PageModuleTypeDetails>(pageModuleTypeId);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        #endregion

        #endregion
    }
}