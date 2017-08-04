using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoute("page-block-types")]
    public class PageBlockTypesApiController : BaseAdminApiController
    {
        #region private member variables
        
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public PageBlockTypesApiController(
            IQueryExecutor queryExecutor,
            IApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #endregion

        #region routes

        #region queries

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _queryExecutor.GetAllAsync<PageBlockTypeSummary>();
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet("{pageBlockTypeId:int}")]
        public async Task<IActionResult> Get(int pageBlockTypeId)
        {
            var results = await _queryExecutor.GetByIdAsync<PageBlockTypeDetails>(pageBlockTypeId);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        #endregion

        #endregion
    }
}